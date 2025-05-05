
CREATE   PROCEDURE [par].[GetRebateDataWithCredit](@Logins [par].[TypeRebateParamLogins] READONLY, @DateFrom date, @DateTo date
	, @SessionStart time = null, @SessionEnd time = null) 
AS
IF @DateTo < '2022-11-01' SET @SessionStart = null -- turn ON PaidSession FROM this date

-- 5 minutes duration and full account ignorance based on the color
DECLARE @colorExcludeALL int = 2763429
DECLARE @colorExclude5min int = 1262987
DECLARE @minutesStartDate datetime = '2024-02-26'
DECLARE @year2001 datetime = '2001-01-01'
DECLARE @minutesTradeDuration int = 5
DECLARE @timeMinTradeBefore datetime = DATEADD(MINUTE, -@minutesTradeDuration, GETDATE())

-- accounts list per platform type (MT4 only, MT5 & cTrader together)
DECLARE @LoginsExtended [par].[TypeRebateParamLoginsExtended]
SELECT * INTO #LoginsMT4 FROM @LoginsExtended
SELECT * INTO #LoginsMT5 FROM @LoginsExtended
INSERT #LoginsMT4 EXEC [par].[GetRebateParamLoginsExtended] @Logins, 1, @colorExcludeALL
INSERT #LoginsMT5 EXEC [par].[GetRebateParamLoginsExtended] @Logins, 0, @colorExcludeALL

SELECT -- [par].[GetRebateDataWithCredit]
T.ServerId
, T.[Login] AS [Login]
, CASE 
	WHEN T.[ServerId] = 48 AND (U.[Group] LIKE 'BB%RC%' OR U.[Group] LIKE 'BB%ELT%') THEN 6 -- Raw Spreads ON Real8
	WHEN T.[ServerId] = 1 AND U.[Group] LIKE 'BB%PRO%' THEN 6 -- Pro Real1 = Raw Spreads
	WHEN T.[ServerId] = 55 AND (U.[Group] LIKE 'real\%RC%' OR U.[Group] LIKE 'real\%ELT%') THEN 6 -- mt5-2 = Raw Spreads
	WHEN S.[SymbolExecutionMode] = 2 AND T.[ServerId] IN (1,7,47) THEN 5 
	ELSE T.[PlatformType] END 
AS PlatformType
, ISNULL(G.Currency,'USD') AS [AccountCurrency]
, T.[Symbol]
, T.ClientMembershipId
, T.DateFrom
, T.DateTo
, T.isClose
, T.OrdersCount
, T.Lots AS Lots -- *LotsMultiplier in the code
, T.LotsUSD AS LotsUSD -- *LotsMultiplier
, T.VolumeUSD
, (CASE S.IsUSDNominated WHEN 1 THEN T.Lots ELSE T.LotsUSD END) * PipMultiplier AS RebateUSDPerPip -- *LotsMultiplier
, CASE T.Lots WHEN 0 THEN 1 ELSE CASE S.IsUSDNominated WHEN 1 THEN NULL ELSE T.LotsUSD / T.Lots END END AS AveRateUSD -- not sure it's correct to set it to 1
,T.CommissionUSD
,T.CommissionLotsUSD -- = lots * rate_AccountCurrency_To_USD
,T.LotsWithSpreadUSD * S.ContractSize AS RebateBySpreadUSD --  RebateBySpreadUSD = LotsWithSpreadUSD * ContractSize * %rebate = (TradeSizeLot / 2) * ConversionToUSD * Spread * ContractSize * %rebate
,S.MarginCalculationMode AS CalcMode
,S.SymbolExecutionMode AS ExecutionMode
,S.IsUSDNominated AS InUSD
,S.PipMultiplier
,S.ContractSize
FROM ---- MT4 trades opened in this period (they can be both opened AND closed)
(
SELECT 0 as isClose
                  ,L.[ServerId]
                  ,L.[Login]
                  ,L.[PlatformType]
                  ,T.[Symbol]
                  ,L.[ClientMembershipId]

                  --,L.DateFrom, L.DateTo
                  ,SUM(CASE WHEN OpenPrice<>0 AND OpenedVolumeUSD<>0 AND OpenedVolumeTradeCurrency<>0 THEN 1 ELSE 0 END) as OrdersCount
                  ,SUM(CASE WHEN OpenPrice<>0 AND OpenedVolumeUSD<>0 AND OpenedVolumeTradeCurrency<>0 THEN [Lot] ELSE 0 END) as Lots

                  -- we need Quote currency to USD rate.
                  -- example for EURJPY:
                  -- EUR/USD Rate = (OpenedVolumeUSD / OpenedVolumeTradeCurrency)
                  -- EUR/JPY Rate = OpenPrice
                  -- so, JPY/USD Rate = 1 / OpenPrice * (OpenedVolumeUSD / OpenedVolumeTradeCurrency)
                  -- here is Lots * JPY/USD Rate, we use it to pay X JPY per Lot (or per pip)
                  ,SUM(CASE WHEN OpenPrice<>0 AND OpenedVolumeUSD<>0 AND OpenedVolumeTradeCurrency<>0 THEN [Lot] / (OpenPrice * (OpenedVolumeTradeCurrency / OpenedVolumeUSD )) ELSE 0 END) AS LotsUSD
                  ,SUM(CASE WHEN OpenPrice<>0 AND OpenedVolumeUSD<>0 AND OpenedVolumeTradeCurrency<>0 THEN OpenedVolumeUSD ELSE 0 END) AS VolumeUSD
                  --,SUM(Lot * OpenedVolumeUSD / (Volume * OpenPrice)) AS LotsOpenedUSD_GOLD
				  
				  -- Commissions ---
				  -- before 2017.10.31:
                  --,SUM([CommissionUSD]) AS CommissionUSD
				  --,SUM(CASE WHEN [Commission]=0 THEN Lot ELSE Lot * [CommissionUSD] / [Commission] END) AS CommissionLotsUSD
				  -- after 2017.10.31:
				  ,SUM(CASE OpenedVolumeAccountCurrency WHEN 0 THEN 0 ELSE Commission * OpenedVolumeUSD/OpenedVolumeAccountCurrency END) as CommissionUSD
				  ,SUM(CASE WHEN Commission=0 OR OpenedVolumeAccountCurrency = 0 THEN Lot ELSE Lot * OpenedVolumeUSD/OpenedVolumeAccountCurrency END) AS CommissionLotsUSD
				  -- Commissions --
                  
				  -- Bridge's Spreads
				  -- LotsWithSpreadUSD = (TradeSizeLot / 2) * ConversionToUSD * Spread
				  ,SUM(CASE WHEN T.OpenPrice = 0 OR T.OpenedVolumeTradeCurrency = 0 OR T.OpenedVolumeUSD = 0 
					    THEN 0 
					      ELSE COALESCE(B1.[SubmissionAsk] - B1.[SubmissionBid], T.[OpenSpread], 0) * T.[Lot] / Case S.IsUSDNominated WHEN 1 THEN 1 ELSE (T.OpenPrice * (T.OpenedVolumeTradeCurrency / T.OpenedVolumeUSD )) END
							* COALESCE(BH.[CreditRatio],L.[CreditRatioDefault])
						END) AS LotsWithSpreadUSD
				  ,MAX(IIF(L.DateFrom > @DateFrom, L.DateFrom, @DateFrom)) as DateFrom -- GREATEST
				  ,MIN(IIF(L.DateTo < @DateTo, L.DateTo, @DateTo)) as DateTo           -- LEAST
                                
					FROM #LoginsMT4 L 
					INNER JOIN [dbo].[TradeRecord] T  with (NOLOCK, index ([TradeRecord_SrvLoginOpenTimeCom_IxPar])) ON T.ServerId = L.ServerId AND T.[Login] = L.[Login] 
					INNER JOIN [par].[ViewRebateSymbols] S WITH (NOLOCK) ON S.[ServerId] = T.[ServerId] AND S.[Symbol] = T.[Symbol] AND S.[Multiply] > 0 -- valid symbols only
					LEFT OUTER JOIN [dbo].[BalanceHistoryMT4] BH WITH (NOLOCK) ON BH.[ServerId] = T.[ServerId] AND BH.[Login] = T.[Login] AND BH.[Order] = T.[Order] AND BH.[IsClose] = 0
					LEFT OUTER JOIN [par].[BridgeRequests] B1 WITH(NOLOCK) ON B1.ServerID = T.ServerId AND B1.[OrderIdValue] = T.[Order] AND B1.[IsClosing] = 0
								AND B1.[Login] = T.[Login] AND (B1.[PluginTimestamp] BETWEEN (DATEADD(MINUTE, -2, T.OpenTime)) AND DATEADD(MINUTE, 2, T.OpenTime)) 

					--AND isOpen IN (0,1) 
					WHERE
					    [TradeCommand] IN (0,1) 
					AND [TradeRecordState] <> 6 -- no deleted
					AND T.[Comment] <> N'-//-'
					AND [OpenTime] >= L.DateFrom AND [OpenTime] < DATEADD(DAY, 1, L.DateTo)
					AND [OpenTime] >= @DateFrom AND [OpenTime] < DATEADD(DAY, 1, @DateTo)
					AND (@SessionStart IS NULL OR CAST([OpenTime] AS time) BETWEEN @SessionStart AND @SessionEnd) -- paid session 
					AND 
					(   
						L.[UserColor] <> @colorExclude5min -- apply the rule for this color only
						OR [OpenTime] < @minutesStartDate  -- don't apply for old trades opened before @minutesStartDate
						OR ([OpenTime] < @timeMinTradeBefore AND [CloseTime] < @year2001)    -- trade is still open, duration is more than @minutesTradeDuration
						OR [CloseTime] >= DATEADD(MINUTE, @minutesTradeDuration, [OpenTime]) -- trade is closed, duration is more than @minutesTradeDuration
					)
                --AND OpenedVolumeUSD<>0
                --WHERE OpenPrice<>0 AND OpenedVolumeTradeCurrency<>0 AND OpenedVolumeUSD <> 0
                GROUP BY L.[ServerId], L.[Login], L.[PlatformType], T.[Symbol], L.[ClientMembershipId]
                
---- MT4 closed trades
UNION ALL
SELECT 1 as isClose 
                  ,L.[ServerId]
                  ,L.[Login]
                  ,L.[PlatformType]
                  ,T.[Symbol]
                  ,L.[ClientMembershipId]
                  --, L.DateFrom, L.DateTo
				  -- 2017.12.14 fixed 'divide by zero' problem when ClosePrice = 0 (fake MT5 trades)
                  ,SUM(CASE WHEN ClosePrice<>0 AND ClosedVolumeTradeCurrency<>0 AND ClosedVolumeUSD <> 0 THEN 1 ELSE 0 END) as OrdersCount
                  ,SUM(CASE WHEN ClosePrice<>0 AND ClosedVolumeTradeCurrency<>0 AND ClosedVolumeUSD <> 0 THEN [Lot] ELSE 0 END) as Lots -- CHECK!!!
                  ,SUM(CASE WHEN ClosePrice<>0 AND ClosedVolumeTradeCurrency<>0 AND ClosedVolumeUSD <> 0 THEN [Lot] / (ClosePrice * (ClosedVolumeTradeCurrency / ClosedVolumeUSD )) ELSE 0 END) AS LotsUSD
                  ,SUM(CASE WHEN ClosePrice<>0 AND ClosedVolumeTradeCurrency<>0 AND ClosedVolumeUSD <> 0 THEN ClosedVolumeUSD ELSE 0 END) AS VolumeClosedUSD
				  -- Commissions ---
				  -- before 2017.10.31:
                  -- ,SUM([CommissionUSD]) AS CommissionUSD
                  -- ,SUM(CASE WHEN [Commission]=0 THEN Lot ELSE Lot * [CommissionUSD] / [Commission] END) AS CommissionLotsUSD 
				  -- after 2017.10.31:
				  ,SUM(CASE OpenedVolumeAccountCurrency WHEN 0 THEN 0 ELSE Commission * OpenedVolumeUSD/OpenedVolumeAccountCurrency END) as CommissionUSD
				  ,SUM(CASE WHEN Commission=0 OR OpenedVolumeAccountCurrency = 0 THEN Lot ELSE Lot * OpenedVolumeUSD/OpenedVolumeAccountCurrency END) AS CommissionLotsUSD
				  -- Commissions --

				  -- Bridge's Spreads
				  ,SUM(CASE WHEN T.ClosePrice = 0 OR T.ClosedVolumeTradeCurrency = 0 OR T.ClosedVolumeUSD = 0 
					    THEN 0
					   ELSE COALESCE(B1.[SubmissionAsk] - B1.[SubmissionBid], T.[CloseSpread], 0) * T.[Lot] / CASE S.IsUSDNominated WHEN 1 THEN 1 ELSE (T.ClosePrice * (T.ClosedVolumeTradeCurrency / T.ClosedVolumeUSD )) END
						* COALESCE(BH.[CreditRatio],L.[CreditRatioDefault])
						END) AS LotsWithSpreadUSD
				  ,MAX(IIF(L.DateFrom > @DateFrom, L.DateFrom, @DateFrom)) as DateFrom -- GREATEST
				  ,MIN(IIF(L.DateTo < @DateTo, L.DateTo, @DateTo)) as DateTo           -- LEAST

                FROM #LoginsMT4 L 
				INNER JOIN [dbo].[TradeRecord] T with (NOLOCK, index ([TradeRecord_SrvLoginCloseTimeCom_IxPar])) ON T.ServerId = L.ServerId AND T.[Login] = L.[Login] 
				INNER JOIN [par].[ViewRebateSymbols] S WITH (NOLOCK) ON S.[ServerId] = T.[ServerId] AND S.[Symbol] = T.[Symbol] AND S.[Multiply] > 0 -- valid symbols only
				LEFT OUTER JOIN [dbo].[BalanceHistoryMT4] BH WITH (NOLOCK) ON BH.[ServerId] = T.[ServerId] AND BH.[Login] = T.[Login] AND BH.[Order] = T.[Order] AND BH.[IsClose] = 1
				LEFT OUTER JOIN [par].[BridgeRequests] B1 WITH(NOLOCK) ON B1.ServerID = T.ServerId AND B1.[OrderIdValue] = T.[Order] AND B1.[IsClosing] = 1
							AND B1.[Login] = T.[Login]	AND (B1.[PluginTimestamp] BETWEEN (DATEADD(MINUTE, -2, T.CloseTime)) AND DATEADD(MINUTE, 2, T.CloseTime)) 

                WHERE 
				    [TradeCommand] IN (0,1) 
				AND [TradeRecordState] <> 6 -- exclude deleted records
				AND T.[Comment] <> N'-//-'
				AND [CloseTime] > @year2001 -- definetely closed
                AND [CloseTime] >= L.DateFrom AND [CloseTime] < DATEADD(day, 1, L.DateTo)
                AND [CloseTime] >= @DateFrom AND [CloseTime] < DATEADD(day, 1, @DateTo)
				AND (@SessionStart IS NULL OR CAST([CloseTime] AS time) BETWEEN @SessionStart AND @SessionEnd)
				AND 
				(   L.[UserColor] <> @colorExclude5min -- apply the rule for this color only
					OR [CloseTime] < @minutesStartDate -- ignore old trades before @minutesStartDate
					OR [CloseTime] >= DATEADD(MINUTE, @minutesTradeDuration, [OpenTime]) -- trade is closed, duration is more than @minutesTradeDuration
				)
                GROUP BY L.[ServerId], L.[Login], L.[PlatformType], T.[Symbol], L.[ClientMembershipId]

-------- MT5 AND cTrader - Open trades
UNION ALL 
SELECT 0 as isClose
                  ,L.[ServerId]
                  ,L.[Login]
                  ,L.[PlatformType]
                  ,T.[Symbol]
                  ,L.[ClientMembershipId]


                  --,L.DateFrom, L.DateTo
                  ,SUM(CASE WHEN OpenPrice<>0 AND OpenedVolumeUSD<>0 AND OpenedVolumeTradeCurrency<>0 THEN 1 ELSE 0 END) as OrdersCount
                  ,SUM(CASE WHEN OpenPrice<>0 AND OpenedVolumeUSD<>0 AND OpenedVolumeTradeCurrency<>0 THEN [Lot] ELSE 0 END) as Lots

				  -- cTrader CreditRatio = 1, MT5 CreditRatio is 0 or calculated from BalanceHistory (see CreditRatioDefault calc field)
                  ,SUM(CASE WHEN OpenPrice<>0 AND OpenedVolumeUSD<>0 AND OpenedVolumeTradeCurrency<>0 THEN [Lot] / (OpenPrice * (OpenedVolumeTradeCurrency / OpenedVolumeUSD )) ELSE 0 END) AS LotsUSD
                  ,SUM(CASE WHEN OpenPrice<>0 AND OpenedVolumeUSD<>0 AND OpenedVolumeTradeCurrency<>0 THEN OpenedVolumeUSD ELSE 0 END) AS VolumeUSD
				  ,SUM(CASE OpenedVolumeAccountCurrency WHEN 0 THEN 0 ELSE Commission * OpenedVolumeUSD/OpenedVolumeAccountCurrency END) as CommissionUSD
				  ,SUM(CASE WHEN Commission=0 OR OpenedVolumeAccountCurrency = 0 THEN Lot ELSE Lot * OpenedVolumeUSD/OpenedVolumeAccountCurrency END) AS CommissionLotsUSD
                  
				  -- Bridge's Spreads
				  -- LotsWithSpreadUSD = (TradeSizeLot / 2) * ConversionToUSD * Spread
				  ,SUM(CASE WHEN T.OpenPrice = 0 OR T.OpenedVolumeTradeCurrency = 0 OR T.OpenedVolumeUSD = 0 
					    THEN 0 
					   ELSE COALESCE(T.OpenSpread, B2.[SubmissionAsk] - B2.[SubmissionBid], 0) * T.[Lot] / CASE S.IsUSDNominated WHEN 1 THEN 1 ELSE (T.OpenPrice * (T.OpenedVolumeTradeCurrency / T.OpenedVolumeUSD )) END
							* COALESCE(BH.[CreditRatio],L.[CreditRatioDefault])
						END) AS LotsWithSpreadUSD
				  ,MAX(IIF(L.DateFrom > @DateFrom, L.DateFrom, @DateFrom)) as DateFrom -- GREATEST
				  ,MIN(IIF(L.DateTo < @DateTo, L.DateTo, @DateTo)) as DateTo           -- LEAST
                                
					FROM #LoginsMT5 L 
					INNER JOIN [dbo].[TradeRecord] T with (NOLOCK, index ([TradeRecord_SrvLoginOpenTimeCom_IxPar])) ON T.ServerId = L.ServerId AND T.[Login] = L.[Login] 
					INNER JOIN [par].[ViewRebateSymbols] S WITH (NOLOCK) ON S.[ServerId] = T.[ServerId] AND S.[Symbol] = T.[Symbol] AND S.[Multiply] > 0 -- valid symbols only
					LEFT OUTER JOIN [dbo].[BalanceHistoryMT5] BH WITH (NOLOCK) ON L.[PlatformType] = 2 AND BH.[ServerId] = T.[ServerId] AND BH.[Login] = T.[Login] AND BH.[Deal] = T.[OpenDeal]
					LEFT OUTER JOIN [par].[BridgeRequests] B2 WITH(NOLOCK) ON B2.ServerID = T.ServerId AND CAST(B2.[RequestId] AS nvarchar(20)) = T.[LpOpenOrderId]
						AND B2.[Login] = T.[Login] AND (B2.[PluginTimestamp] BETWEEN (DATEADD(MINUTE, -2, T.OpenTime)) AND DATEADD(MINUTE, 2, T.OpenTime))
                                
					WHERE 
					    [TradeCommand] IN (0,1) 
					AND [TradeRecordState] <> 6 -- no deleted
					AND T.[Comment] <> N'-//-'
					AND [OpenTime] >= L.DateFrom AND [OpenTime] < DATEADD(DAY, 1, L.DateTo)
					AND [OpenTime] >= @DateFrom AND [OpenTime] < DATEADD(DAY, 1, @DateTo)
					AND (@SessionStart IS NULL OR CAST([OpenTime] AS time) BETWEEN @SessionStart AND @SessionEnd)
					AND 
					(   
						L.[UserColor] <> @colorExclude5min -- apply the rule for this color only
						OR [OpenTime] < @minutesStartDate  -- don't apply for old trades opened before @minutesStartDate
						OR ([OpenTime] < @timeMinTradeBefore AND [CloseTime] < @year2001)    -- trade is still open, duration is more than @minutesTradeDuration
						OR [CloseTime] >= DATEADD(MINUTE, @minutesTradeDuration, [OpenTime]) -- trade is closed, duration is more than @minutesTradeDuration
					)
					GROUP BY L.[ServerId], L.[Login], L.[PlatformType], T.[Symbol], L.[ClientMembershipId]
-------- MT5 AND cTrader - Closed trades
UNION ALL
SELECT 1 as isClose 
                  ,L.[ServerId]
                  ,L.[Login]
                  ,L.[PlatformType]
                  ,T.[Symbol]
                  ,L.[ClientMembershipId]

                  ,SUM(CASE WHEN ClosePrice<>0 AND ClosedVolumeTradeCurrency<>0 AND ClosedVolumeUSD <> 0 THEN 1 ELSE 0 END) as OrdersCount
                  ,SUM(CASE WHEN ClosePrice<>0 AND ClosedVolumeTradeCurrency<>0 AND ClosedVolumeUSD <> 0 THEN [Lot] ELSE 0 END) as Lots 
                  ,SUM(CASE WHEN ClosePrice<>0 AND ClosedVolumeTradeCurrency<>0 AND ClosedVolumeUSD <> 0 THEN [Lot] / (ClosePrice * (ClosedVolumeTradeCurrency / ClosedVolumeUSD )) ELSE 0 END) AS LotsUSD
                  ,SUM(CASE WHEN ClosePrice<>0 AND ClosedVolumeTradeCurrency<>0 AND ClosedVolumeUSD <> 0 THEN ClosedVolumeUSD ELSE 0 END) AS VolumeClosedUSD
				  ,SUM(CASE OpenedVolumeAccountCurrency WHEN 0 THEN 0 ELSE Commission * OpenedVolumeUSD/OpenedVolumeAccountCurrency END) as CommissionUSD
				  ,SUM(CASE WHEN Commission=0 OR OpenedVolumeAccountCurrency = 0 THEN Lot ELSE Lot * OpenedVolumeUSD/OpenedVolumeAccountCurrency END) AS CommissionLotsUSD

				  -- Bridge's Spreads
				  ,SUM(CASE WHEN T.ClosePrice = 0 OR T.ClosedVolumeTradeCurrency = 0 OR T.ClosedVolumeUSD = 0 
					    THEN 0
					   ELSE COALESCE(T.CloseSpread, B2.[SubmissionAsk] - B2.[SubmissionBid], 0) * T.[Lot] / CASE S.IsUSDNominated WHEN 1 THEN 1 ELSE (T.ClosePrice * (T.ClosedVolumeTradeCurrency / T.ClosedVolumeUSD )) END
							* COALESCE(BH.[CreditRatio],L.[CreditRatioDefault])
						END) AS LotsWithSpreadUSD
				  ,MAX(IIF(L.DateFrom > @DateFrom, L.DateFrom, @DateFrom)) as DateFrom -- GREATEST
				  ,MIN(IIF(L.DateTo < @DateTo, L.DateTo, @DateTo)) as DateTo           -- LEAST

				  FROM #LoginsMT5 L 
				  INNER JOIN [dbo].[TradeRecord] T with (NOLOCK, index ([TradeRecord_SrvLoginCloseTimeCom_IxPar])) ON T.ServerId = L.ServerId AND T.[Login] = L.[Login] 
				  INNER JOIN [par].[ViewRebateSymbols] S WITH (NOLOCK) ON S.[ServerId] = T.[ServerId] AND S.[Symbol] = T.[Symbol] AND S.[Multiply] > 0 -- valid symbols only
				  LEFT OUTER JOIN [dbo].[BalanceHistoryMT5] BH WITH (NOLOCK) ON L.[PlatformType] = 2 AND BH.[ServerId] = T.[ServerId] AND BH.[Login] = T.[Login] AND BH.[Deal] = T.[CloseDeal]
  				  LEFT OUTER JOIN [par].[BridgeRequests] B2 WITH(NOLOCK) ON B2.ServerID = T.ServerId AND CAST(B2.[RequestId] AS nvarchar(20)) = T.[LpCloseOrderId]
					AND B2.[Login] = T.[Login]	AND (B2.[PluginTimestamp] BETWEEN (DATEADD(MINUTE, -2, T.CloseTime)) AND DATEADD(MINUTE, 2, T.CloseTime)) 

                WHERE 
				    [TradeCommand] IN (0,1) 
				AND [TradeRecordState] <> 6 -- no deleted 
				AND [TradeRecordState] <> 5 -- exclude Closed By trades
				AND T.[Comment] <> N'-//-'
				AND [CloseTime] > @year2001 -- definetely closed
                AND [CloseTime] >= L.DateFrom AND [CloseTime] < DATEADD(day, 1, L.DateTo)
                AND [CloseTime] >= @DateFrom AND [CloseTime] < DATEADD(day, 1, @DateTo)
				AND (@SessionStart IS NULL OR CAST([CloseTime] AS time) BETWEEN @SessionStart AND @SessionEnd)
				AND 
				(   L.[UserColor] <> @colorExclude5min -- apply the rule for this color only
					OR [CloseTime] < @minutesStartDate -- ignore old trades before @minutesStartDate
					OR [CloseTime] >= DATEADD(MINUTE, @minutesTradeDuration, [OpenTime]) -- trade is closed, duration is more than @minutesTradeDuration
				)
				GROUP BY L.[ServerId], L.[Login], L.[PlatformType], T.[Symbol], L.[ClientMembershipId]
) T 
INNER JOIN [par].[ViewRebateSymbols] S WITH (NOLOCK) ON S.[ServerId] = T.[ServerId] AND S.[Symbol] = T.[Symbol] AND S.[Multiply] > 0 -- valid symbols only
LEFT OUTER JOIN [dbo].[UserRecord] U WITH (NOLOCK) ON U.ServerId=T.[ServerId] AND U.[Login]=T.[Login]
LEFT OUTER JOIN [dbo].[ConGroup] G WITH (NOLOCK) ON G.ServerId=U.[ServerId] AND G.[Group]=U.[Group]
ORDER BY T.PlatformType, T.ServerId, T.[Login], T.[Symbol], T.DateFrom, T.isClose

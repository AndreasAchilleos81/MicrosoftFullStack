export default function toggleMenuVisibility() {
    const menuButton = document.querySelector(".header__menu-button");
    const nav = document.querySelector(".header__nav");
    menuButton.addEventListener("click", function() {
        nav.classList.toggle("header__nav--active");  
    });
}
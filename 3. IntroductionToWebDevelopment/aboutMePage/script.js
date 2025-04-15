const toggleBtn = document.getElementById('toggleBtn');
toggleBtn.addEventListener('click', () => {
  toggleMenu();
});

function toggleMenu() {
  const menu = document.getElementById('menu');
  menu.classList.toggle('active');
} 

const navLinks = document.querySelectorAll('.menu a');
navLinks.forEach(link => {
  link.addEventListener('click', event => {
    event.preventDefault(); // Prevent default anchor click behavior
    const targetId = event.target.getAttribute('href').substring(1); // Get the target section ID
    const targetSection = document.getElementById(targetId);
    if (targetSection) {
      const targetPosition = targetSection.getBoundingClientRect().top + window.scrollY;
      const startPosition = window.scrollY;
      const distance = targetPosition - startPosition;
      const duration = 1000; // Duration in milliseconds
      let startTime = null;

      function animation(currentTime) {
        if (startTime === null) startTime = currentTime;
        const timeElapsed = currentTime - startTime;
        const run = ease(timeElapsed, startPosition, distance, duration);
        window.scrollTo(0, run);
        if (timeElapsed < duration) requestAnimationFrame(animation);
      }

      function ease(timeElapsed, startPosition, distance, duration) {
        timeElapsed /= duration / 2;
        if (timeElapsed < 1) return distance / 2 * timeElapsed * timeElapsed + startPosition;
        timeElapsed--;
        return -distance / 2 * (timeElapsed * (timeElapsed - 2) - 1) + startPosition;
      }

      requestAnimationFrame(animation);
    }
  });
});

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

const searchInput = document.getElementById('project-search');
const projectCards = document.querySelectorAll('.projects-section .card');
const categories = document.querySelectorAll('.projects-section .category');

// Function to filter projects based on the search term
function filterProjects(searchTerm) {
  projectCards.forEach(card => {
    const title = card.querySelector('h3').textContent.toLowerCase();
    const description = card.querySelector('p').textContent.toLowerCase();
    const category = card.querySelector('.category').textContent.toLowerCase();


    if (title.includes(searchTerm) || description.includes(searchTerm) || category.includes(searchTerm)) {
      card.style.display = ''; // Show the card
    } else {
      card.style.display = 'none'; // Hide the card
    }
  });
}

// Event listener for the search input
searchInput.addEventListener('input', () => {
  const searchTerm = searchInput.value.toLowerCase();
  filterProjects(searchTerm);
});

// light box effect for projects code
const projectImages = document.querySelectorAll('.projects-section .card img');
const lightbox = document.getElementById('lightbox');
const lightboxImage = document.getElementById('lightbox-image');
const closeBtn = document.getElementById('lightbox-close');
const prevBtn = document.getElementById('prev-btn');
const nextBtn = document.getElementById('next-btn');

let currentIndex = 0;

// Function to open the lightbox
function openLightbox(index) {
  const visibleImages = Array.from(projectImages).filter(img => img.closest('.card').style.display !== 'none');
  currentIndex = index;
  lightboxImage.src = visibleImages[currentIndex].src;
  lightbox.style.display = 'flex';
}

// Function to close the lightbox
function closeLightbox() {
  lightbox.style.display = 'none';
}

// Function to navigate to the previous image
function showPrevious() {
  const visibleImages = Array.from(projectImages).filter(img => img.closest('.card').style.display !== 'none');
  currentIndex = (currentIndex - 1 + visibleImages.length) % visibleImages.length;
  lightboxImage.src = visibleImages[currentIndex].src;
}

// Function to navigate to the next image
function showNext() {
  const visibleImages = Array.from(projectImages).filter(img => img.closest('.card').style.display !== 'none');
  currentIndex = (currentIndex + 1) % visibleImages.length;
  lightboxImage.src = visibleImages[currentIndex].src;
}

// Event listeners for project images
projectImages.forEach((img, index) => {
  img.addEventListener('click', () => {
    const visibleImages = Array.from(projectImages).filter(img => img.closest('.card').style.display !== 'none');
    const visibleIndex = visibleImages.indexOf(img);
    openLightbox(visibleIndex);
  });
});

// Event listeners for lightbox controls
closeBtn.addEventListener('click', closeLightbox);
prevBtn.addEventListener('click', showPrevious);
nextBtn.addEventListener('click', showNext);

// Close lightbox when clicking outside the image
lightbox.addEventListener('click', (event) => {
  if (event.target === lightbox) {
    closeLightbox();
  }
});


const form = document.getElementById('contact-form');
const nameInput = document.getElementById('name');
const emailInput = document.getElementById('email');
const messageInput = document.getElementById('message');

const nameError = document.getElementById('name-error');
const emailError = document.getElementById('email-error');
const messageError = document.getElementById('message-error');

// Function to validate name
function validateName() {
  if (nameInput.value.trim() === '' || nameInput.value.length < 3) {
    nameError.textContent = 'Name is required and must be at least 3 characters long.';
    nameError.style.display = 'block';
    return false;
  } else {
    nameError.style.display = 'none';
    return true;
  }
}

// Function to validate email
function validateEmail() {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (emailInput.value.trim() === '') {
    emailError.textContent = 'Email is required.';
    emailError.style.display = 'block';
    return false;
  } else if (!emailRegex.test(emailInput.value.trim())) {
    emailError.textContent = 'Please enter a valid email address.';
    emailError.style.display = 'block';
    return false;
  } else {
    emailError.style.display = 'none';
    return true;
  }
}

// Function to validate message
function validateMessage() {
  if (messageInput.value.trim() === '') {
    messageError.textContent = 'Message is required.';
    messageError.style.display = 'block';
    return false;
  } else {
    messageError.style.display = 'none';
    return true;
  }
}

// Real-time validation
nameInput.addEventListener('input', validateName);
emailInput.addEventListener('input', validateEmail);
messageInput.addEventListener('input', validateMessage);

// Form submission validation
form.addEventListener('submit', (event) => {
  const isNameValid = validateName();
  const isEmailValid = validateEmail();
  const isMessageValid = validateMessage();

  if (!isNameValid || !isEmailValid || !isMessageValid) {
    event.preventDefault(); // Prevent form submission if validation fails
  }
});
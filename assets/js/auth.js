// DentLink - Auth JS
document.addEventListener('DOMContentLoaded', () => {
  const loginForm = document.getElementById('loginForm');
  if (loginForm) {
    loginForm.addEventListener('submit', (e) => {
      e.preventDefault();
      const role = document.getElementById('role')?.value;
      const routes = {
        patient: '../patient/dashboard.html',
        student: '../student/dashboard.html',
        admin: '../admin/dashboard.html'
      };
      if (role && routes[role]) {
        window.location.href = routes[role];
      }
    });
  }

  document.querySelectorAll('.password-toggle').forEach(btn => {
    btn.addEventListener('click', () => {
      const input = document.getElementById(btn.dataset.target);
      if (!input) return;
      const isPassword = input.type === 'password';
      input.type = isPassword ? 'text' : 'password';
      btn.innerHTML = isPassword
        ? '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19m-6.72-1.07a3 3 0 11-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>'
        : '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>';
    });
  });

  const fileInput = document.getElementById('studentId');
  const fileUpload = document.getElementById('fileUpload');
  const fileName = document.getElementById('fileName');
  if (fileInput && fileUpload && fileName) {
    fileInput.addEventListener('change', () => {
      if (fileInput.files.length > 0) {
        fileUpload.classList.add('has-file');
        fileName.textContent = fileInput.files[0].name;
      } else {
        fileUpload.classList.remove('has-file');
        fileName.textContent = '';
      }
    });
  }

  const doctorSignupForm = document.getElementById('doctorSignupForm');
  if (doctorSignupForm) {
    doctorSignupForm.addEventListener('submit', (e) => {
      const password = document.getElementById('password').value;
      const confirmPassword = document.getElementById('confirmPassword').value;
      if (password !== confirmPassword) {
        e.preventDefault();
        alert('Passwords do not match. Please try again.');
      }
    });
  }

  const patientSignupForm = document.getElementById('patientSignupForm');
  if (patientSignupForm) {
    patientSignupForm.addEventListener('submit', (e) => {
      const password = document.getElementById('password').value;
      const confirmPassword = document.getElementById('confirmPassword').value;
      const city = document.getElementById('city');

      if (password !== confirmPassword) {
        e.preventDefault();
        alert('Passwords do not match. Please try again.');
        return;
      }

      if (city && (city.disabled || !city.value)) {
        e.preventDefault();
        alert('Please select a city for your governorate.');
      }
    });
  }
});

// DentLink - Student JS
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('.case-btn-request, .request-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      alert('Your request has been sent successfully!');
      window.location.href = 'my-requests.html';
    });
  });

  const searchInput = document.getElementById('casesSearch');
  const chips = document.querySelectorAll('.cases-chip');
  const cards = document.querySelectorAll('.case-card');
  const emptyState = document.getElementById('casesEmpty');
  let activeFilter = 'all';

  function filterCases() {
    const query = searchInput?.value.toLowerCase().trim() || '';
    let visibleCount = 0;

    cards.forEach(card => {
      const category = card.dataset.category || '';
      const text = card.textContent.toLowerCase();
      const matchesFilter = activeFilter === 'all' || category === activeFilter;
      const matchesSearch = !query || text.includes(query);
      const visible = matchesFilter && matchesSearch;

      card.classList.toggle('hidden', !visible);
      if (visible) visibleCount++;
    });

    if (emptyState) {
      emptyState.classList.toggle('visible', visibleCount === 0);
    }
  }

  chips.forEach(chip => {
    chip.addEventListener('click', () => {
      chips.forEach(c => c.classList.remove('active'));
      chip.classList.add('active');
      activeFilter = chip.dataset.filter || 'all';
      filterCases();
    });
  });

  if (searchInput) {
    searchInput.addEventListener('input', filterCases);
  }

  document.querySelectorAll('.session-arrive-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      const card = btn.closest('.session-card-full');
      const badge = card?.querySelector('.session-badge');
      const actions = card?.querySelector('.session-card-actions');
      if (badge) {
        badge.textContent = 'in progress';
        badge.className = 'session-badge session-badge-progress';
      }
      if (actions) {
        actions.innerHTML = `
          <button class="session-btn-complete session-complete-btn">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="20 6 9 17 4 12"/></svg>
            Complete Session
          </button>
          <span class="session-present-badge">Patient present</span>`;
        actions.querySelector('.session-complete-btn')?.addEventListener('click', completeSession);
      }
    });
  });

  document.querySelectorAll('.session-complete-btn').forEach(btn => {
    btn.addEventListener('click', completeSession);
  });

  const profileForm = document.getElementById('profileForm');
  if (profileForm) {
    profileForm.addEventListener('submit', (e) => {
      e.preventDefault();
      alert('Profile saved successfully!');
    });
  }
});

function completeSession(e) {
  const card = e.currentTarget.closest('.session-card-full');
  const badge = card?.querySelector('.session-badge');
  const actions = card?.querySelector('.session-card-actions');
  if (badge) {
    badge.textContent = 'completed';
    badge.className = 'session-badge session-badge-completed';
  }
  if (actions) {
    actions.innerHTML = `
      <span class="session-done-msg">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 11-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
        Session completed successfully
      </span>`;
  }
}

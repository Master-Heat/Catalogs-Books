/* ============================================================
   BOOK PAGE — book.js
   Reads ?id= from URL → loads book data → renders correct view
   Admin vs User decided by localStorage user.role
   ============================================================ */

// ── MOCK DATA ─────────────────────────────────────────────────
// This mirrors the same IDs used in books.js (homepage)
// Replace fetch() calls with real API when backend is ready
const BOOK_CATALOGUE = {
  1: {
    id: 1, type: "book",
    title: "To Kill a Mockingbird", author: "Harper Lee",
    cover: "../assets/To_Kill_a_Mockingbird_(first_edition_cover).jpg",
    description: "The unforgettable novel of a childhood in a sleepy Southern town and the crisis of conscience that rocked it. 'To Kill A Mockingbird' became both an instant bestseller and a critical success when it was first published in 1960. It went on to win the Pulitzer Prize in 1961 and was later made into an Academy Award-winning film, also a classic.",
    readUrl: "#", downloadUrl: "#",
    info: { "Published Year": "1960", "Publisher": "J.B. Lippincott & Co.", "Pages": "324", "Language": "English", "ISBN": "978-0-06-112008-4" },
    avgRating: 4.2, ratingsCount: 1234,
    reviews: [
      { id: 1, username: "BookLover42", rating: 5, date: "March 1, 2026",    text: "An absolute masterpiece! This book changed my perspective on so many things." },
      { id: 2, username: "ReaderJane",  rating: 3, date: "February 28, 2026",text: "Beautifully written with unforgettable characters. A must-read classic." },
      { id: 3, username: "ClassicFan",  rating: 5, date: "February 26, 2026",text: "One of the best books I've ever read. The themes are still relevant today." },
    ],
  },

  2: {
    id: 2, type: "book",
    title: "1984", author: "George Orwell",
    cover: null,
    description: "A dystopian social science fiction novel and cautionary tale. The novel is set in Airstrip One, a province of the superstate Oceania in a world of perpetual war, omnipresent government surveillance, and propaganda.",
    readUrl: "#", downloadUrl: "#",
    info: { "Published Year": "1949", "Publisher": "Secker & Warburg", "Pages": "328", "Language": "English", "ISBN": "978-0-452-28423-4" },
    avgRating: 4.5, ratingsCount: 3201,
    reviews: [
      { id: 1, username: "PoliSciGuy", rating: 5, date: "March 4, 2026", text: "More relevant today than ever. A chilling masterpiece." },
    ],
  },

  // Series example
  8: {
    id: 8, type: "series",
    title: "The Lord of the Rings", author: "J.R.R. Tolkien",
    cover: null,
    description: "An epic high-fantasy novel written by English author and scholar J. R. R. Tolkien. The story began as a sequel to Tolkien's 1937 fantasy novel The Hobbit, but eventually developed into a much larger work. Written in stages between 1937 and 1949, The Lord of the Rings is one of the best-selling novels ever written.",
    readUrl: "#", downloadUrl: "#",
    volumes: [
      { id: 1, title: "Volume 1: The Fellowship of the Ring", pages: "423 pages", cover: null, readUrl: "#", downloadUrl: "#" },
      { id: 2, title: "Volume 2: The Two Towers",             pages: "352 pages", cover: null, readUrl: "#", downloadUrl: "#" },
      { id: 3, title: "Volume 3: The Return of the King",     pages: "416 pages", cover: null, readUrl: "#", downloadUrl: "#" },
    ],
    info: { "Published Year": "1954–1955", "Publisher": "Allen & Unwin", "Language": "English", "Volumes": "3" },
    avgRating: 4.5, ratingsCount: 5678,
    reviews: [
      { id: 1, username: "FantasyFan", rating: 5, date: "March 5, 2026", text: "The greatest fantasy series ever written. Tolkien's world-building is unmatched." },
      { id: 2, username: "EpicReader", rating: 4, date: "March 3, 2026", text: "A timeless masterpiece that has influenced countless works of fantasy." },
    ],
  },
};

// ── STATE ─────────────────────────────────────────────────────
let currentBook  = null;
let isAdmin      = false;
let userRating   = 0;   // header interactive rating
let reviewRating = 0;   // review-form rating
let deleteTarget = null; // { type: 'book' | 'review', reviewId? }

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", () => {
  const params = new URLSearchParams(window.location.search);
  const bookId = parseInt(params.get("id"), 10);

  isAdmin = getAdminStatus();

  loadBook(bookId);
  initInteractiveStars();
  initModals();
  initReviewSubmit();
  initVolumeToggle();
});

// ── AUTH ──────────────────────────────────────────────────────
function getAdminStatus() {
  try {
    const user = JSON.parse(localStorage.getItem("user") || "{}");
    return user.role === "admin";
  } catch {
    return false;
  }
}

// ── LOAD BOOK ─────────────────────────────────────────────────
function loadBook(id) {
  /* ── Swap this block for a real API call later ──
     fetch(`/api/books/${id}`)
       .then(r => r.json())
       .then(data => renderBook(data))
       .catch(() => showNotFound());
  */

  const book = BOOK_CATALOGUE[id];
  if (!book) { showNotFound(); return; }

  currentBook = book;
  renderBook(book);
}

function showNotFound() {
  document.getElementById("bookTitle").textContent = "Book not found.";
  document.getElementById("bookAuthor").textContent = "";
  document.getElementById("bookDescription").textContent =
    "This book does not exist or has been removed.";
}

// ── RENDER ────────────────────────────────────────────────────
function renderBook(book) {
  document.title = `${book.title} — Catalogs`;

  // Basic fields
  document.getElementById("bookTitle").textContent       = book.title;
  document.getElementById("bookAuthor").textContent      = book.author;
  document.getElementById("bookDescription").textContent = book.description;

  // Cover
  const coverEl = document.getElementById("bookCover");
  if (book.cover) {
    coverEl.innerHTML = `<img src="${book.cover}" alt="${book.title} cover">`;
  } else {
    coverEl.innerHTML = `<span>${book.type === "series" ? "Series Cover" : "Book Cover"}</span>`;
  }

  // Buttons (admin vs user)
  renderActionButtons();

  // Read / Download
  document.getElementById("readBtn").onclick     = () => window.open(book.readUrl, "_blank");
  document.getElementById("downloadBtn").onclick = () => window.open(book.downloadUrl, "_blank");

  // Volumes (series only)
  if (book.type === "series" && Array.isArray(book.volumes)) {
    renderVolumes(book.volumes);
    document.getElementById("volumesSection").style.display = "block";
  }

  // Info table
  renderInfoTable(book);

  // Average rating stars
  renderAvgStars(book.avgRating, book.ratingsCount);

  // Reviews
  renderReviews(book.reviews);
}

// ── ACTION BUTTONS ────────────────────────────────────────────
function renderActionButtons() {
  const container = document.getElementById("actionButtons");
  container.innerHTML = "";

  if (isAdmin) {
    const delBtn = document.createElement("button");
    delBtn.className   = "btn-admin-delete";
    delBtn.textContent = "Delete";
    delBtn.onclick     = () => openDeleteModal("book");

    const editBtn = document.createElement("button");
    editBtn.className   = "btn-admin-edit";
    editBtn.textContent = "Edit";
    editBtn.onclick     = openEditModal;

    container.append(delBtn, editBtn);
  } else {
    const addBtn = document.createElement("button");
    addBtn.className = "btn-add-list";
    addBtn.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"
           fill="none" stroke="currentColor" stroke-width="2">
        <line x1="12" y1="5" x2="12" y2="19"/>
        <line x1="5"  y1="12" x2="19" y2="12"/>
      </svg>
      Add to List`;
    addBtn.onclick = handleAddToList;
    container.appendChild(addBtn);
  }
}

// ── VOLUMES ───────────────────────────────────────────────────
function renderVolumes(volumes) {
  document.getElementById("volumesLabel").textContent = `Volumes (${volumes.length})`;

  const list = document.getElementById("volumesList");
  list.innerHTML = "";

  volumes.forEach(vol => {
    const item = document.createElement("div");
    item.className = "volume-item";

    item.innerHTML = `
      <div class="volume-thumb">
        ${vol.cover
          ? `<img src="${vol.cover}" alt="cover">`
          : `<span>Vol.</span>`}
      </div>
      <div class="volume-info">
        <div class="volume-title">${vol.title}</div>
        <div class="volume-pages">${vol.pages}</div>
      </div>
      <div class="volume-actions">
        <button class="vol-btn vol-btn-read">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24"
               fill="none" stroke="currentColor" stroke-width="2">
            <path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/>
            <path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/>
          </svg>
          Read
        </button>
        <button class="vol-btn vol-btn-download">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24"
               fill="none" stroke="currentColor" stroke-width="2">
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
            <polyline points="7 10 12 15 17 10"/>
            <line x1="12" y1="15" x2="12" y2="3"/>
          </svg>
          Download
        </button>
      </div>`;

    // Bind volume buttons after HTML is set
    item.querySelector(".vol-btn-read").onclick     = () => window.open(vol.readUrl, "_blank");
    item.querySelector(".vol-btn-download").onclick = () => window.open(vol.downloadUrl, "_blank");

    list.appendChild(item);
  });
}

function initVolumeToggle() {
  const header = document.getElementById("volumesHeader");
  const list   = document.getElementById("volumesList");
  const toggle = document.getElementById("volumesToggle");
  if (!header) return;

  // Start open
  list.classList.add("open");
  toggle.classList.add("open");

  header.addEventListener("click", () => {
    list.classList.toggle("open");
    toggle.classList.toggle("open");
  });
}

// ── INFO TABLE ────────────────────────────────────────────────
function renderInfoTable(book) {
  document.getElementById("infoTitle").textContent =
    book.type === "series" ? "Series Information" : "Book Information";

  const table = document.getElementById("infoTable");
  table.innerHTML = "";

  Object.entries(book.info).forEach(([label, value]) => {
    const row = document.createElement("div");
    row.className = "info-row";
    row.innerHTML = `
      <span class="info-label">${label}</span>
      <span class="info-value">${value}</span>`;
    table.appendChild(row);
  });
}

// ── AVERAGE STARS ─────────────────────────────────────────────
function renderAvgStars(avg, count) {
  const container = document.getElementById("avgStars");
  container.innerHTML = "";

  for (let i = 1; i <= 5; i++) {
    const star = document.createElement("span");
    star.className = "star";
    star.innerHTML = "&#9733;";
    if (i <= Math.round(avg)) star.classList.add("filled");
    container.appendChild(star);
  }

  document.getElementById("ratingsCount").textContent =
    `(${count.toLocaleString()} ratings)`;
}

// ── INTERACTIVE STARS ─────────────────────────────────────────
function initInteractiveStars() {
  setupStarGroup(
    document.querySelectorAll("#userStarRating .star"),
    val => { userRating = val; }
  );

  setupStarGroup(
    document.querySelectorAll(".review-star"),
    val => { reviewRating = val; }
  );
}

function setupStarGroup(stars, onSelect) {
  stars.forEach(star => {
    star.addEventListener("mouseover", () => paintStars(stars, +star.dataset.value, "hover"));
    star.addEventListener("mouseout",  () => paintStars(stars, currentActiveValue(stars), "active"));
    star.addEventListener("click", () => {
      onSelect(+star.dataset.value);
      paintStars(stars, +star.dataset.value, "active");
    });
  });
}

function paintStars(stars, value, cls) {
  stars.forEach(s => {
    s.classList.remove("active", "hover");
    if (+s.dataset.value <= value) s.classList.add(cls);
  });
}

function currentActiveValue(stars) {
  let v = 0;
  stars.forEach(s => { if (s.classList.contains("active")) v = +s.dataset.value; });
  return v;
}

// ── REVIEWS ───────────────────────────────────────────────────
function renderReviews(reviews) {
  const list = document.getElementById("reviewsList");
  list.innerHTML = "";

  if (!reviews || reviews.length === 0) {
    list.innerHTML = `<p style="font-size:13px;color:#777;font-family:'Georgia',serif;">
      No reviews yet. Be the first!</p>`;
    return;
  }

  reviews.forEach(review => {
    const item = document.createElement("div");
    item.className = "review-item";
    item.dataset.reviewId = review.id;

    // Build star HTML
    let starsHTML = "";
    for (let i = 1; i <= 5; i++) {
      starsHTML += `<span class="star${i <= review.rating ? " filled" : ""}">&#9733;</span>`;
    }

    item.innerHTML = `
      <div class="review-top">
        <div class="review-left">
          <span class="review-username">${review.username}</span>
          <div class="review-stars">${starsHTML}</div>
        </div>
        <div class="review-right">
          <span class="review-date">${review.date}</span>
          ${isAdmin
            ? `<button class="btn-delete-review">Delete</button>`
            : ""}
        </div>
      </div>
      <p class="review-text">${review.text}</p>`;

    // Bind admin delete button after HTML is inserted
    if (isAdmin) {
      item.querySelector(".btn-delete-review").onclick =
        () => openDeleteModal("review", review.id);
    }

    list.appendChild(item);
  });
}

// ── REVIEW SUBMIT ─────────────────────────────────────────────
function initReviewSubmit() {
  document.getElementById("submitReview").addEventListener("click", () => {
    const text = document.getElementById("reviewText").value.trim();

    if (!text) {
      alert("Please write a review before submitting.");
      return;
    }
    if (reviewRating === 0) {
      alert("Please select a star rating.");
      return;
    }

    /* ── Replace with real API call ──
       fetch(`/api/books/${currentBook.id}/reviews`, {
         method: "POST",
         headers: { "Content-Type": "application/json" },
         body: JSON.stringify({ rating: reviewRating, text })
       }).then(r => r.json()).then(newReview => { ... });
    */

    const newReview = {
      id: Date.now(),
      username: "You",   // Replace with logged-in user
      rating: reviewRating,
      date: new Date().toLocaleDateString("en-US",
        { month: "long", day: "numeric", year: "numeric" }),
      text,
    };

    currentBook.reviews.unshift(newReview);
    renderReviews(currentBook.reviews);

    // Reset form
    document.getElementById("reviewText").value = "";
    reviewRating = 0;
    paintStars(document.querySelectorAll(".review-star"), 0, "active");
  });
}

// ── ADD TO LIST ───────────────────────────────────────────────
function handleAddToList() {
  /* ── Replace with real API call ──
     fetch(`/api/user/list`, {
       method: "POST",
       headers: { "Content-Type": "application/json" },
       body: JSON.stringify({ bookId: currentBook.id })
     });
  */
  alert(`"${currentBook.title}" added to your list!`);
}

// ── MODALS ────────────────────────────────────────────────────
function initModals() {
  // Edit
  document.getElementById("closeEditModal").onclick = closeEditModal;
  document.getElementById("cancelEdit").onclick     = closeEditModal;
  document.getElementById("saveEdit").onclick       = saveEdit;

  // Delete
  document.getElementById("closeDeleteModal").onclick = closeDeleteModal;
  document.getElementById("cancelDelete").onclick     = closeDeleteModal;
  document.getElementById("confirmDelete").onclick    = confirmDelete;

  // Close on backdrop click
  document.getElementById("editModal").addEventListener("click", e => {
    if (e.target === e.currentTarget) closeEditModal();
  });
  document.getElementById("deleteModal").addEventListener("click", e => {
    if (e.target === e.currentTarget) closeDeleteModal();
  });
}

/* EDIT ─────────────────────────────────────────── */
function openEditModal() {
  const body = document.getElementById("editModalBody");
  body.innerHTML = "";

  // Core fields
  const coreFields = [
    { key: "title",       label: "Title",       type: "input",    value: currentBook.title },
    { key: "author",      label: "Author",      type: "input",    value: currentBook.author },
    { key: "description", label: "Description", type: "textarea", value: currentBook.description },
  ];

  // Info fields (dynamic from book data)
  const infoFields = Object.entries(currentBook.info).map(([label, value]) => ({
    key: `info__${label}`,
    label,
    type: "input",
    value,
    isInfo: true,
    infoLabel: label,
  }));

  [...coreFields, ...infoFields].forEach(field => {
    const wrap = document.createElement("div");
    wrap.className = "edit-field";

    const lbl = document.createElement("label");
    lbl.htmlFor     = `edit_${field.key}`;
    lbl.textContent = field.label;

    const el = document.createElement(field.type === "textarea" ? "textarea" : "input");
    el.id    = `edit_${field.key}`;
    el.value = field.value;
    if (field.isInfo) {
      el.dataset.isInfo    = "true";
      el.dataset.infoLabel = field.infoLabel;
    } else {
      el.dataset.bookKey = field.key;
    }

    wrap.append(lbl, el);
    body.appendChild(wrap);
  });

  document.getElementById("editModal").style.display = "flex";
}

function closeEditModal() {
  document.getElementById("editModal").style.display = "none";
}

function saveEdit() {
  document.querySelectorAll("#editModalBody input, #editModalBody textarea")
    .forEach(el => {
      const val = el.value.trim();
      if (!val) return;

      if (el.dataset.isInfo) {
        currentBook.info[el.dataset.infoLabel] = val;
      } else {
        currentBook[el.dataset.bookKey] = val;
      }
    });

  /* ── Replace with real API call ──
     fetch(`/api/books/${currentBook.id}`, {
       method: "PATCH",
       headers: { "Content-Type": "application/json" },
       body: JSON.stringify(currentBook)
     });
  */

  renderBook(currentBook);
  closeEditModal();
}

/* DELETE ────────────────────────────────────────── */
function openDeleteModal(type, reviewId = null) {
  deleteTarget = { type, reviewId };

  document.getElementById("deleteModalText").textContent =
    type === "book"
      ? "Are you sure you want to delete this book? This action cannot be undone."
      : "Are you sure you want to delete this review?";

  document.getElementById("deleteModal").style.display = "flex";
}

function closeDeleteModal() {
  document.getElementById("deleteModal").style.display = "none";
  deleteTarget = null;
}

function confirmDelete() {
  if (!deleteTarget) return;

  if (deleteTarget.type === "book") {
    /* ── Replace with real API call ──
       fetch(`/api/books/${currentBook.id}`, { method: "DELETE" })
         .then(() => window.location.href = "homepage.html");
    */
    alert("Book deleted.");
    window.location.href = "homepage.html";

  } else if (deleteTarget.type === "review") {
    /* ── Replace with real API call ──
       fetch(`/api/books/${currentBook.id}/reviews/${deleteTarget.reviewId}`,
         { method: "DELETE" });
    */
    currentBook.reviews = currentBook.reviews.filter(
      r => r.id !== deleteTarget.reviewId
    );
    renderReviews(currentBook.reviews);
    closeDeleteModal();
  }
}
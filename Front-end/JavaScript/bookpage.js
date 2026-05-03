/* ============================================================
   BOOK PAGE — bookpage.js
   Connects to:
     GET  /api/Books/Authorized/{id}     → book details
     POST /api/Reviews/submit            → submit review
     GET  /api/BookLists/Authorized/MyLists   → get user lists
     POST /api/BookLists/Authorized/AddBookToList → add to list
   ============================================================ */

// ── STATE ─────────────────────────────────────────────────────
let currentBook  = null;
let isAdmin      = false;
let reviewRating = 0;
let deleteTarget = null;
let userLists    = [];   // Stores user's book lists

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("jwt_token");

    // Redirect to login if no token
    if (!token) {
        window.location.href = "./login.html";
        return;
    }

    // Check admin from saved user info
    isAdmin = getAdminStatus();

    const params = new URLSearchParams(window.location.search);
    const bookId = parseInt(params.get("id"), 10);

    if (!bookId) {
        showNotFound();
        return;
    }

    await loadBook(bookId, token);
    initInteractiveStars();
    initModals();
    initReviewSubmit();
    initVolumeToggle();
    showAdminPanelIfAdmin();
});

// ── AUTH ──────────────────────────────────────────────────────
function getAdminStatus() {
    try {
        const user = JSON.parse(localStorage.getItem("user") || "{}");
        // Role saved as "Admin" from JWT claim (capital A from C# backend)
        return user.role === "Admin" || user.role === "admin";
    } catch {
        return false;
    }
}

function showAdminPanelIfAdmin() {
    if (isAdmin) {
        const navActions = document.querySelector(".nav-actions");
        if (navActions) {
            const adminLink = document.createElement("a");
            adminLink.href = "./adminaccount.html";
            adminLink.className = "btn-admin";
            adminLink.innerHTML = `
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M12 2L6 6V10C6 14.42 8.46 18.36 12 20.77C15.54 18.36 18 14.42 18 10V6L12 2Z"/>
                    <path d="M12 12C13.1 12 14 11.1 14 10C14 8.9 13.1 8 12 8C10.9 8 10 8.9 10 10C10 11.1 10.9 12 12 12Z"/>
                </svg>
                <span>Admin</span>
            `;
            navActions.insertBefore(adminLink, navActions.querySelector(".btn-account"));
        }

        const mobileMenu = document.querySelector(".mobile-menu");
        if (mobileMenu) {
            const mobileAdminLink = document.createElement("a");
            mobileAdminLink.href = "./adminaccount.html";
            mobileAdminLink.className = "mobile-btn-admin";
            mobileAdminLink.innerHTML = `
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M12 2L6 6V10C6 14.42 8.46 18.36 12 20.77C15.54 18.36 18 14.42 18 10V6L12 2Z"/>
                    <path d="M12 12C13.1 12 14 11.1 14 10C14 8.9 13.1 8 12 8C10.9 8 10 8.9 10 10C10 11.1 10.9 12 12 12Z"/>
                </svg>
                Admin
            `;
            const mobileAccount = mobileMenu.querySelector(".mobile-btn-account");
            mobileMenu.insertBefore(mobileAdminLink, mobileAccount);
        }
    }
}

function getAuthHeaders() {
    const token = localStorage.getItem("jwt_token");
    return {
        "Authorization": `Bearer ${token}`,
        "Content-Type":  "application/json"
    };
}

// ── LOAD BOOK FROM API ────────────────────────────────────────
async function loadBook(id, token) {
    try {
        const response = await fetch(
            `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.BOOKS_AUTHORIZED}/${id}`,
            {
                method:  "GET",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type":  "application/json"
                }
            }
        );

        if (response.status === 401) {
            // Session expired
            localStorage.removeItem("jwt_token");
            localStorage.removeItem("user");
            window.location.href = "./login.html";
            return;
        }

        if (response.status === 404) {
            showNotFound();
            return;
        }

        if (!response.ok) {
            throw new Error(`Server error: ${response.status}`);
        }

        const data = await response.json();
        currentBook = data;
        renderBook(data);

    } catch (error) {
        showNotFound("Failed to load book details. Please try again later.");
    }
}

function showNotFound(message = "Book not found.") {
    document.getElementById("bookTitle").textContent = message;
    document.getElementById("bookAuthor").textContent = "";
    document.getElementById("bookDescription").textContent =
        "This book does not exist or has been removed.";
}

// ── RENDER BOOK ───────────────────────────────────────────────
/*  API BookDetailsDTO shape (based on BooksController):
    {
      bookID, title, description,
      author: { authorID, name },
      category: { categoryID, name },
      series: { seriesID, name, volumes: [...] } | null,
      coverImageLink, coverAlt,
      canDownload, downloadLink,
      publicationDate, pagesCount,
      averageRating, ratingsCount,
      reviews: [{ reviewID, userName, reviewText, ratingValue, createdAt }]
    }
*/
function renderBook(book) {
    document.title = `${book.title} — Catalogs`;

    // Basic fields
    document.getElementById("bookTitle").textContent       = book.title       || "";
    document.getElementById("bookAuthor").textContent      = book.author?.name || "";
    document.getElementById("bookDescription").textContent = book.description  || "";

    // Cover
    const coverEl = document.getElementById("bookCover");
    if (book.coverImageLink) {
        coverEl.innerHTML =
            `<img src="${book.coverImageLink}" alt="${book.coverAlt || book.title}">`;
    } else {
        coverEl.innerHTML =
            `<span>${book.series ? "Series Cover" : "Book Cover"}</span>`;
    }

    // Action buttons (admin edit/delete vs user add-to-list)
    renderActionButtons();

    // Read / Download buttons
    const readBtn     = document.getElementById("readBtn");
    const downloadBtn = document.getElementById("downloadBtn");

    if (book.downloadLink) {
        readBtn.onclick     = () => window.open(book.downloadLink, "_blank");
        downloadBtn.onclick = () => window.open(book.downloadLink, "_blank");
    }

    // Hide download if not allowed
    if (!book.canDownload) {
        downloadBtn.style.display = "none";
    }

    // Series volumes
    if (book.series?.volumes?.length > 0) {
        renderVolumes(book.series.volumes);
        document.getElementById("volumesSection").style.display = "block";
    }

    // Info table
    renderInfoTable(book);

    // Average rating
    renderAvgStars(book.averageRating || 0, book.ratingsCount || 0);

    // Reviews
    renderReviews(book.reviews || []);
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
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
                 viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
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
    document.getElementById("volumesLabel").textContent =
        `Volumes (${volumes.length})`;

    const list = document.getElementById("volumesList");
    list.innerHTML = "";

    volumes.forEach(vol => {
        const item = document.createElement("div");
        item.className = "volume-item";
        item.style.cursor = "pointer";

        // Clicking a volume navigates to its own book page
        item.addEventListener("click", () => {
            window.location.href = `bookpage.html?id=${vol.bookID}`;
        });

        item.innerHTML = `
            <div class="volume-thumb">
                ${vol.coverImageLink
                    ? `<img src="${vol.coverImageLink}" alt="cover">`
                    : `<span>Vol.</span>`}
            </div>
            <div class="volume-info">
                <div class="volume-title">${vol.title}</div>
                <div class="volume-pages">${vol.pagesCount ? vol.pagesCount + " pages" : ""}</div>
            </div>`;

        list.appendChild(item);
    });
}

function initVolumeToggle() {
    const header = document.getElementById("volumesHeader");
    const list   = document.getElementById("volumesList");
    const toggle = document.getElementById("volumesToggle");
    if (!header) return;

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
        book.series ? "Series Information" : "Book Information";

    const table = document.getElementById("infoTable");
    table.innerHTML = "";

    // Build info rows from API data
    const rows = [
        ["Category",         book.category?.name],
        ["Author",           book.author?.name],
        ["Published",        book.publicationDate
                               ? new Date(book.publicationDate)
                                     .getFullYear()
                               : null],
        ["Pages",            book.pagesCount],
        ["Series",           book.series?.name],
    ];

    rows.forEach(([label, value]) => {
        if (!value) return; // Skip empty rows

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
        document.querySelectorAll(".review-star"),
        val => { reviewRating = val; }
    );
}

function setupStarGroup(stars, onSelect) {
    stars.forEach(star => {
        star.addEventListener("mouseover", () =>
            paintStars(stars, +star.dataset.value, "hover"));
        star.addEventListener("mouseout",  () =>
            paintStars(stars, currentActiveValue(stars), "active"));
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
/* API review shape:
   { reviewID, userName, reviewText, ratingValue, createdAt }
*/
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
        item.className     = "review-item";
        item.dataset.reviewId = review.reviewID;

        // Build stars
        let starsHTML = "";
        for (let i = 1; i <= 5; i++) {
            starsHTML += `<span class="star${i <= Math.round(review.ratingValue)
                ? " filled" : ""}">&#9733;</span>`;
        }

        // Format date
        const dateStr = review.createdAt
            ? new Date(review.createdAt).toLocaleDateString("en-US", {
                month: "long", day: "numeric", year: "numeric"
              })
            : "";

        item.innerHTML = `
            <div class="review-top">
                <div class="review-left">
                    <span class="review-username">${escapeHTML(review.userName || "")}</span>
                    <div class="review-stars">${starsHTML}</div>
                </div>
                <div class="review-right">
                    <span class="review-date">${dateStr}</span>
                    ${isAdmin
                        ? `<button class="btn-delete-review">Delete</button>`
                        : ""}
                </div>
            </div>
            <p class="review-text">${escapeHTML(review.reviewText || "")}</p>`;

        if (isAdmin) {
            item.querySelector(".btn-delete-review").onclick =
                () => openDeleteModal("review", review.reviewID);
        }

        list.appendChild(item);
    });
}

// ── SUBMIT REVIEW ─────────────────────────────────────────────
function initReviewSubmit() {
    document.getElementById("submitReview").addEventListener("click", async () => {
        const text = document.getElementById("reviewText").value.trim();

        if (!text) {
            alert("Please write a review before submitting.");
            return;
        }
        if (reviewRating === 0) {
            alert("Please select a star rating.");
            return;
        }

        const submitBtn = document.getElementById("submitReview");
        submitBtn.disabled    = true;
        submitBtn.textContent = "Submitting...";

        try {
            // POST /api/Reviews/submit?BookID=&ReviewText=&RateValue=
            const url = `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.REVIEWS_SUBMIT}` +
                `?BookID=${currentBook.bookID}` +
                `&ReviewText=${encodeURIComponent(text)}` +
                `&RateValue=${reviewRating}`;

            const response = await fetch(url, {
                method:  "POST",
                headers: getAuthHeaders()
            });

            if (response.status === 401) {
                alert("Session expired. Please log in again.");
                window.location.href = "./login.html";
                return;
            }

            if (!response.ok) {
                const err = await response.text();
                throw new Error(err);
            }

            // ✅ Refresh book to show updated review
            const token = localStorage.getItem("jwt_token");
            await loadBook(currentBook.bookID, token);

            // Reset form
            document.getElementById("reviewText").value = "";
            reviewRating = 0;
            paintStars(
                document.querySelectorAll(".review-star"), 0, "active");

        } catch (error) {
            console.error("Review submit failed:", error);
            alert("Failed to submit review. Please try again.");
        } finally {
            submitBtn.disabled    = false;
            submitBtn.textContent = "Submit Review";
        }
    });
}

// ── ADD TO LIST ───────────────────────────────────────────────
async function handleAddToList() {
    try {
        // 1. Fetch user's lists
        const response = await fetch(
            `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.BOOK_LISTS_AUTHORIZED}/MyLists`,
            { method: "GET", headers: getAuthHeaders() }
        );

        if (response.status === 401) {
            window.location.href = "./login.html";
            return;
        }

        userLists = await response.json();
        openListModal();

    } catch (error) {
        console.error("Failed to load lists:", error);
        alert("Could not load your lists. Please try again.");
    }
}

// ── LIST MODAL ────────────────────────────────────────────────
function openListModal() {
    // Remove existing list modal if any
    const existing = document.getElementById("listModal");
    if (existing) existing.remove();

    const modal = document.createElement("div");
    modal.id        = "listModal";
    modal.className = "modal-overlay";
    modal.style.display = "flex";

    const listsHTML = userLists.length > 0
        ? userLists.map(list => `
            <button class="list-choice-btn" data-list-id="${list.listID}">
                ${escapeHTML(list.listName)}
            </button>`).join("")
        : `<p style="font-size:13px;color:#777;">
               You have no lists yet. Create one below.</p>`;

    modal.innerHTML = `
        <div class="modal">
            <div class="modal-header">
                <h3>Add to List</h3>
                <button class="modal-close" id="closeListModal">&times;</button>
            </div>
            <div class="modal-body">
                <div class="lists-container">${listsHTML}</div>
                <div class="new-list-row">
                    <input type="text" id="newListName"
                           placeholder="New list name..." 
                           class="new-list-input">
                    <button class="btn-create-list" id="createListBtn">
                        Create
                    </button>
                </div>
            </div>
        </div>`;

    document.body.appendChild(modal);

    // Close modal
    modal.querySelector("#closeListModal").onclick =
        () => modal.remove();
    modal.addEventListener("click", e => {
        if (e.target === modal) modal.remove();
    });

    // Select existing list
    modal.querySelectorAll(".list-choice-btn").forEach(btn => {
        btn.addEventListener("click", () =>
            addBookToList(+btn.dataset.listId, modal));
    });

    // Create new list
    modal.querySelector("#createListBtn").onclick = async () => {
        const name = modal.querySelector("#newListName").value.trim();
        if (!name) return;
        await createListThenAdd(name, modal);
    };
}

async function addBookToList(listID, modal) {
    try {
        const response = await fetch(
            `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.BOOK_LISTS_AUTHORIZED}/AddBookToList`,
            {
                method:  "POST",
                headers: getAuthHeaders(),
                body:    JSON.stringify({
                    listID:  listID,
                    bookID:  currentBook.bookID
                })
            }
        );

        if (!response.ok) {
            const err = await response.json();
            throw new Error(err.message || "Failed to add book.");
        }

        modal.remove();
        showToast(`"${currentBook.title}" added to list!`);

    } catch (error) {
        showToast(error.message || "Failed to add book to list.", "error");
    }
}

async function createListThenAdd(name, modal) {
    try {
        // Create new list
        const createRes = await fetch(
            `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.BOOK_LISTS_AUTHORIZED}/CreateList`,
            {
                method:  "POST",
                headers: getAuthHeaders(),
                body:    JSON.stringify(name)
            }
        );

        if (!createRes.ok) {
            const err = await createRes.json();
            throw new Error(err.message || "Failed to create list.");
        }

        // Reload lists and add book
        const listsRes = await fetch(
            `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.BOOK_LISTS_AUTHORIZED}/MyLists`,
            { method: "GET", headers: getAuthHeaders() }
        );
        userLists = await listsRes.json();

        // Find the newly created list by name
        const newList = userLists.find(l => l.listName === name);
        if (newList) {
            await addBookToList(newList.listID, modal);
        }

    } catch (error) {
        showToast(error.message || "Failed to create list.", "error");
    }
}

// ── TOAST NOTIFICATION ────────────────────────────────────────
function showToast(message, type = "success") {
    const existing = document.getElementById("bookToast");
    if (existing) existing.remove();

    const toast = document.createElement("div");
    toast.id = "bookToast";
    Object.assign(toast.style, {
        position:     "fixed",
        bottom:       "30px",
        left:         "50%",
        transform:    "translateX(-50%)",
        background:   type === "success" ? "#1a1a1a" : "#c0392b",
        color:        "#fff",
        padding:      "12px 24px",
        borderRadius: "4px",
        fontSize:     "13px",
        zIndex:       "9999",
        boxShadow:    "0 4px 12px rgba(0,0,0,0.2)",
    });
    toast.textContent = message;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = "0";
        toast.style.transition = "opacity 0.3s";
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// ── MODALS (Edit / Delete) ────────────────────────────────────
function initModals() {
    document.getElementById("closeEditModal").onclick   = closeEditModal;
    document.getElementById("cancelEdit").onclick       = closeEditModal;
    document.getElementById("saveEdit").onclick         = saveEdit;
    document.getElementById("closeDeleteModal").onclick = closeDeleteModal;
    document.getElementById("cancelDelete").onclick     = closeDeleteModal;
    document.getElementById("confirmDelete").onclick    = confirmDelete;

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

    const fields = [
        { key: "title",       label: "Title",       tag: "input",    value: currentBook.title },
        { key: "description", label: "Description", tag: "textarea", value: currentBook.description },
    ];

    fields.forEach(field => {
        const wrap = document.createElement("div");
        wrap.className = "edit-field";

        const lbl = document.createElement("label");
        lbl.htmlFor     = `edit_${field.key}`;
        lbl.textContent = field.label;

        const el = document.createElement(field.tag);
        el.id              = `edit_${field.key}`;
        el.value           = field.value || "";
        el.dataset.bookKey = field.key;

        wrap.append(lbl, el);
        body.appendChild(wrap);
    });

    document.getElementById("editModal").style.display = "flex";
}

function closeEditModal() {
    document.getElementById("editModal").style.display = "none";
}

function saveEdit() {
    // Note: Wire up to PATCH /api/books/{id} when admin edit endpoint is ready
    document.querySelectorAll("#editModalBody input, #editModalBody textarea")
        .forEach(el => {
            const val = el.value.trim();
            if (!val) return;
            currentBook[el.dataset.bookKey] = val;
        });

    renderBook(currentBook);
    closeEditModal();
    showToast("Changes saved locally. API update coming soon.");
}

/* DELETE ────────────────────────────────────────── */
function openDeleteModal(type, reviewId = null) {
    deleteTarget = { type, reviewId };

    document.getElementById("deleteModalText").textContent =
        type === "book"
            ? "Are you sure you want to delete this book? This cannot be undone."
            : "Are you sure you want to delete this review?";

    document.getElementById("deleteModal").style.display = "flex";
}

function closeDeleteModal() {
    document.getElementById("deleteModal").style.display = "none";
    deleteTarget = null;
}

async function confirmDelete() {
    if (!deleteTarget) return;

    if (deleteTarget.type === "book") {
        // Note: Wire up to DELETE /api/books/{id} when endpoint is ready
        showToast("Delete endpoint coming soon.");
        closeDeleteModal();
        // window.location.href = "homepage.html";

    } else if (deleteTarget.type === "review") {
        // Note: Wire up to DELETE /api/Reviews/{id} when endpoint is ready
        showToast("Review delete endpoint coming soon.");
        closeDeleteModal();
    }
}

// ── ESCAPE HELPER ─────────────────────────────────────────────
function escapeHTML(str) {
    if (!str) return "";
    return str
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
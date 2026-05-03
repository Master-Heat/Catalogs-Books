/* ============================================================
   SEARCH OVERLAY — search.js
   Connects to: GET /api/Books/Authorized/search?keyword=
   Requires JWT token in localStorage
   ============================================================ */

// ── STATE ─────────────────────────────────────────────────────
let searchDebounceTimer = null;

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", () => {
    initSearchOverlay();
});

// ── SETUP ─────────────────────────────────────────────────────
function initSearchOverlay() {
    const overlay      = document.getElementById("searchOverlay");
    const overlayInput = document.getElementById("searchOverlayInput");
    const closeBtn     = document.getElementById("searchOverlayClose");

    if (!overlay) return;

    // Hook into ALL navbar search inputs on the page
    const navInputs = document.querySelectorAll(
        "#navSearchInput, #mobileSearchInput"
    );

    navInputs.forEach(input => {
        input.addEventListener("focus", () => openOverlay(input.value));
        input.addEventListener("input", () => openOverlay(input.value));
        input.addEventListener("keydown", (e) => {
            if (e.key === "Escape") closeOverlay();
        });
    });

    // Live search inside overlay
    overlayInput.addEventListener("input", () => {
        const query = overlayInput.value.trim();
        debounceSearch(query);
    });

    closeBtn.addEventListener("click", closeOverlay);

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") closeOverlay();
    });
}

// ── OPEN OVERLAY ──────────────────────────────────────────────
function openOverlay(initialValue = "") {
    const overlay      = document.getElementById("searchOverlay");
    const overlayInput = document.getElementById("searchOverlayInput");

    overlay.classList.add("open");
    document.body.style.overflow = "hidden";

    overlayInput.value = initialValue;
    overlayInput.focus();

    if (initialValue.trim()) {
        runSearch(initialValue.trim());
    } else {
        clearResults();
    }
}

// ── CLOSE OVERLAY ─────────────────────────────────────────────
function closeOverlay() {
    const overlay = document.getElementById("searchOverlay");
    overlay.classList.remove("open");
    document.body.style.overflow = "";

    document.querySelectorAll("#navSearchInput, #mobileSearchInput")
        .forEach(input => input.value = "");

    clearResults();
}

// ── DEBOUNCE ──────────────────────────────────────────────────
function debounceSearch(query) {
    clearTimeout(searchDebounceTimer);

    if (!query) {
        clearResults();
        return;
    }

    searchDebounceTimer = setTimeout(() => runSearch(query), 300);
}

// ── SEARCH — calls real API ───────────────────────────────────
async function runSearch(query) {
    const token = localStorage.getItem("jwt_token");

    // Show loading state
    showLoading();

    // If no token, user is not logged in
    if (!token) {
        showError("Please log in to search for books.");
        return;
    }

    try {
        const url = `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.BOOKS_SEARCH}?keyword=${encodeURIComponent(query)}`;

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (response.status === 401) {
            // Token expired or invalid
            localStorage.removeItem("jwt_token");
            localStorage.removeItem("user");
            showError("Session expired. Please log in again.");
            return;
        }

        if (!response.ok) {
            throw new Error(`Server error: ${response.status}`);
        }

        const results = await response.json();
        renderResults(results, query);

    } catch (error) {
        showError("Search failed. Please try again.");
    }
}

// ── RENDER RESULTS ────────────────────────────────────────────
function renderResults(results, query) {
    const info = document.getElementById("searchOverlayInfo");
    const list = document.getElementById("searchOverlayList");

    list.innerHTML = "";

    if (!results || results.length === 0) {
        info.innerHTML = "";
        list.innerHTML = `
            <div class="search-empty">
                <p class="search-empty-title">No results for "${escapeHTML(query)}"</p>
                <p class="search-empty-sub">Try a different title or author name.</p>
            </div>`;
        return;
    }

    info.innerHTML = `${results.length} result${results.length !== 1 ? "s" : ""} 
        for <span>"${escapeHTML(query)}"</span>`;

    results.forEach(book => {
        const item = createResultItem(book, query);
        list.appendChild(item);
    });
}

function createResultItem(book, query) {
    const item = document.createElement("div");
    item.className = "search-item";

    // API returns: bookID, title, author, coverImageLink, coverAlt
    item.addEventListener("click", () => {
        closeOverlay();
        window.location.href = `bookpage.html?id=${book.bookID}`;
    });

    const coverHTML = book.coverImageLink
        ? `<img src="${book.coverImageLink}" alt="${escapeHTML(book.coverAlt || book.title)}">`
        : "Cover";

    item.innerHTML = `
        <div class="search-cover">${coverHTML}</div>
        <div class="search-info-block">
            <div class="search-book-title">${highlightMatch(book.title, query)}</div>
            <div class="search-book-author">${escapeHTML(book.author || "")}</div>
        </div>`;

    return item;
}

// ── STATE HELPERS ─────────────────────────────────────────────
function showLoading() {
    const list = document.getElementById("searchOverlayList");
    const info = document.getElementById("searchOverlayInfo");
    if (info) info.innerHTML = "";
    if (list) list.innerHTML = `
        <div class="search-empty">
            <p class="search-empty-sub">Searching...</p>
        </div>`;
}

function showError(message) {
    const list = document.getElementById("searchOverlayList");
    const info = document.getElementById("searchOverlayInfo");
    if (info) info.innerHTML = "";
    if (list) list.innerHTML = `
        <div class="search-empty">
            <p class="search-empty-title">${escapeHTML(message)}</p>
        </div>`;
}

function clearResults() {
    const info = document.getElementById("searchOverlayInfo");
    const list = document.getElementById("searchOverlayList");
    if (info) info.innerHTML = "";
    if (list) list.innerHTML = "";
}

// ── HELPERS ───────────────────────────────────────────────────
function highlightMatch(text, query) {
    if (!query || !text) return escapeHTML(text || "");
    const escaped  = escapeHTML(text);
    const escapedQ = escapeHTML(query);
    const regex    = new RegExp(`(${escapedQ.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')})`, "gi");
    return escaped.replace(regex, `<mark>$1</mark>`);
}

function escapeHTML(str) {
    if (!str) return "";
    return str
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
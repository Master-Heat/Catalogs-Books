/* ============================================================
   SEARCH OVERLAY — search.js
   Drop this on any page that has the search overlay HTML.
   It hooks into the existing navbar search inputs and opens
   the overlay with live results as the user types.
   ============================================================ */

// ── MASTER BOOK CATALOGUE ─────────────────────────────────────
// Replace with a real API call when backend is ready:
// fetch("/api/books").then(r => r.json()).then(data => ALL_BOOKS = data)
const ALL_BOOKS = [
    { id: 1,  title: "To Kill a Mockingbird",                 author: "Harper Lee",             cover: "../assets/To_Kill_a_Mockingbird_(first_edition_cover).jpg" },
    { id: 2,  title: "1984",                                  author: "George Orwell",           cover: null },
    { id: 3,  title: "Pride and Prejudice",                   author: "Jane Austen",             cover: null },
    { id: 4,  title: "The Great Gatsby",                      author: "F. Scott Fitzgerald",     cover: null },
    { id: 5,  title: "Moby Dick",                             author: "Herman Melville",         cover: null },
    { id: 6,  title: "The Catcher in the Rye",                author: "J.D. Salinger",           cover: null },
    { id: 7,  title: "Brave New World",                       author: "Aldous Huxley",           cover: null },
    { id: 8,  title: "The Hobbit",                            author: "J.R.R. Tolkien",          cover: null },
    { id: 9,  title: "Fahrenheit 451",                        author: "Ray Bradbury",            cover: null },
    { id: 10, title: "Jane Eyre",                             author: "Charlotte Brontë",        cover: null },
    { id: 11, title: "Wuthering Heights",                     author: "Emily Brontë",            cover: null },
    { id: 12, title: "The Old Man and the Sea",               author: "Ernest Hemingway",        cover: null },
    { id: 13, title: "Crime and Punishment",                  author: "Fyodor Dostoevsky",       cover: null },
    { id: 14, title: "The Brothers Karamazov",                author: "Fyodor Dostoevsky",       cover: null },
    { id: 15, title: "War and Peace",                         author: "Leo Tolstoy",             cover: null },
    { id: 16, title: "Anna Karenina",                         author: "Leo Tolstoy",             cover: null },
    { id: 17, title: "The Iliad",                             author: "Homer",                   cover: null },
    { id: 18, title: "Don Quixote",                           author: "Miguel de Cervantes",     cover: null },
    { id: 19, title: "Ulysses",                               author: "James Joyce",             cover: null },
    { id: 20, title: "The Divine Comedy",                     author: "Dante Alighieri",         cover: null },
    { id: 21, title: "Hamlet",                                author: "William Shakespeare",     cover: null },
    { id: 22, title: "Macbeth",                               author: "William Shakespeare",     cover: null },
    { id: 23, title: "Romeo and Juliet",                      author: "William Shakespeare",     cover: null },
    { id: 24, title: "The Tales of Genji",                    author: "Murasaki Shikibu",        cover: null },
    { id: 25, title: "One Hundred Years of Solitude",         author: "Gabriel García Márquez",  cover: null },
    { id: 26, title: "The Count of Monte Cristo",             author: "Alexandre Dumas",         cover: null },
    { id: 27, title: "Harry Potter and the Sorcerer's Stone", author: "J.K. Rowling",            cover: null },
    { id: 28, title: "Animal Farm",                           author: "George Orwell",           cover: null },
    { id: 29, title: "The Lord of the Rings",                 author: "J.R.R. Tolkien",          cover: null },
];

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
        "#navSearchInput, #mobileSearchInput, .nav-search input, .navbar-search input"
    );

    navInputs.forEach(input => {
        // Open overlay when user starts typing
        input.addEventListener("focus", () => openOverlay(input.value));
        input.addEventListener("input", () => openOverlay(input.value));

        // Keep Enter key working as fallback (opens overlay if not already open)
        input.addEventListener("keydown", (e) => {
            if (e.key === "Escape") closeOverlay();
        });
    });

    // Live search inside overlay
    overlayInput.addEventListener("input", () => {
        const query = overlayInput.value.trim();
        debounceSearch(query);
    });

    // Close button
    closeBtn.addEventListener("click", closeOverlay);

    // Close on Escape key
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

    // Mirror whatever was typed in the navbar into overlay input
    overlayInput.value = initialValue;
    overlayInput.focus();

    // Run search immediately if there's already a value
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

    // Clear all navbar inputs
    document.querySelectorAll(
        "#navSearchInput, #mobileSearchInput, .nav-search input, .navbar-search input"
    ).forEach(input => input.value = "");

    clearResults();
}

// ── DEBOUNCE ──────────────────────────────────────────────────
// Waits 200ms after user stops typing before searching
function debounceSearch(query) {
    clearTimeout(searchDebounceTimer);

    if (!query) {
        clearResults();
        return;
    }

    searchDebounceTimer = setTimeout(() => runSearch(query), 200);
}

// ── SEARCH LOGIC ──────────────────────────────────────────────
function runSearch(query) {
    /* ── Swap for real API call later ──
       fetch(`/api/books/search?q=${encodeURIComponent(query)}`)
         .then(r => r.json())
         .then(results => renderResults(results, query));
    */

    const results = searchBooks(query);
    renderResults(results, query);
}

function searchBooks(query) {
    const q = query.toLowerCase();

    const results = ALL_BOOKS.filter(book =>
        book.title.toLowerCase().includes(q) ||
        book.author.toLowerCase().includes(q)
    );

    // Sort by relevance
    results.sort((a, b) => {
        const score = (book) => {
            const t  = book.title.toLowerCase();
            const au = book.author.toLowerCase();
            if (t.startsWith(q))  return 0;
            if (t.includes(q))    return 1;
            if (au.includes(q))   return 2;
            return 3;
        };
        return score(a) - score(b);
    });

    return results;
}

// ── RENDER ────────────────────────────────────────────────────
function renderResults(results, query) {
    const info = document.getElementById("searchOverlayInfo");
    const list = document.getElementById("searchOverlayList");

    list.innerHTML = "";

    if (results.length === 0) {
        info.innerHTML = "";
        list.innerHTML = `
            <div class="search-empty">
                <p class="search-empty-title">No results for "${escapeHTML(query)}"</p>
                <p class="search-empty-sub">Try a different title or author name.</p>
            </div>`;
        return;
    }

    info.innerHTML =
        `${results.length} result${results.length !== 1 ? "s" : ""} for <span>"${escapeHTML(query)}"</span>`;

    results.forEach(book => {
        const item = createResultItem(book, query);
        list.appendChild(item);
    });
}

function createResultItem(book, query) {
    const item = document.createElement("div");
    item.className = "search-item";

    item.addEventListener("click", () => {
        window.location.href = `bookpage.html?id=${book.id}`;
    });

    const coverHTML = book.cover
        ? `<img src="${book.cover}" alt="${escapeHTML(book.title)}">`
        : "Cover";

    item.innerHTML = `
        <div class="search-cover">${coverHTML}</div>
        <div class="search-info-block">
            <div class="search-book-title">${highlightMatch(book.title, query)}</div>
            <div class="search-book-author">${escapeHTML(book.author)}</div>
        </div>`;

    return item;
}

// ── CLEAR RESULTS ─────────────────────────────────────────────
function clearResults() {
    const info = document.getElementById("searchOverlayInfo");
    const list = document.getElementById("searchOverlayList");
    if (info) info.innerHTML = "";
    if (list) list.innerHTML = "";
}

// ── HELPERS ───────────────────────────────────────────────────
function highlightMatch(text, query) {
    if (!query) return escapeHTML(text);
    const escaped  = escapeHTML(text);
    const escapedQ = escapeHTML(query);
    const regex    = new RegExp(`(${escapedQ})`, "gi");
    return escaped.replace(regex, `<mark>$1</mark>`);
}

function escapeHTML(str) {
    return str
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
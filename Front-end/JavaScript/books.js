/* ============================================================
   BOOKS PAGE — books.js
   Loads featured/popular books for the landing page
   ============================================================ */

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", async () => {
    await loadFeaturedBooks();
});

// ── LOAD FEATURED BOOKS ───────────────────────────────────────
async function loadFeaturedBooks() {
    const token = localStorage.getItem("jwt_token");
    const container = document.getElementById("row-recent");
    
    if (!container) return;

    try {
        // Try to get personalized recommendations if logged in
        if (token) {
            await loadPersonalizedBooks(token, container);
        } else {
            // Load public popular books if not logged in
            await loadPublicPopularBooks(container);
        }
    } catch (error) {
        container.innerHTML = "<p>Could not load books at this time.</p>";
    }
}

// ── LOAD PERSONALIZED BOOKS (authenticated) ───────────────────
async function loadPersonalizedBooks(token, container) {
    try {
        const response = await fetch(
            CONFIG.buildUrl(CONFIG.ENDPOINTS.ACCOUNT_HOMEPAGE),
            {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                }
            }
        );

        if (!response.ok) {
            throw new Error(`API error: ${response.status}`);
        }

        const dashboard = await response.json();
        
        // Use "Popular This Week" if available, otherwise use category recs
        const books = dashboard.popularThisWeek || dashboard.categoryRecs || [];
        
        if (books.length === 0) {
            container.innerHTML = "<p>Start exploring to see featured books!</p>";
            return;
        }

        renderBooks(books, container);
    } catch (error) {
        await loadPublicPopularBooks(container);
    }
}

// ── LOAD PUBLIC POPULAR BOOKS (unauthenticated) ───────────────
async function loadPublicPopularBooks(container) {
    try {
        // Endpoint: GET /api/Books/popular (or similar - check with backend)
        // For now, we'll just show a message if not logged in
        container.innerHTML = `
            <div style="padding: 2rem; text-align: center; color: #000">
                <p>Sign in to see featured books personalized for you!</p>
                <a href="./login.html" style="color: var(--color-primary); text-decoration: underline;">
                    Log In
                </a>
                or
                <a href="./signup.html" style="color: var(--color-primary); text-decoration: underline;">
                    Sign Up
                </a>
            </div>
        `;
    } catch (error) {
        container.innerHTML = "<p>Could not load books at this time.</p>";
    }
}

// ── RENDER BOOKS ──────────────────────────────────────────────
function renderBooks(apiBooks, container) {
    container.innerHTML = "";

    if (!apiBooks || apiBooks.length === 0) {
        container.innerHTML = "<p>No books available.</p>";
        return;
    }

    apiBooks.forEach((apiBook) => {
        const book = mapApiBook(apiBook);
        const card = createBookCard(book);
        container.appendChild(card);
    });
}

// ── MAP API BOOK TO INTERNAL FORMAT ────────────────────────────
function mapApiBook(apiBook) {
    return {
        id: apiBook.bookID,
        title: apiBook.title,
        author: apiBook.author || "Unknown",
        cover: apiBook.coverImageLink || null,
        coverAlt: apiBook.coverAlt || apiBook.title,
    };
}

// ── CREATE BOOK CARD ──────────────────────────────────────────
function createBookCard(book) {
    const card = document.createElement("div");
    card.classList.add("book-card");

    card.style.cursor = "pointer";
    card.addEventListener("click", () => {
        window.location.href = `./bookpage.html?id=${book.id}`;
    });

    // Cover
    const cover = document.createElement("div");
    cover.classList.add("book-cover");

    if (book.cover) {
        const img = document.createElement("img");
        img.src = book.cover;
        img.alt = book.coverAlt;
        cover.appendChild(img);
    } else {
        const placeholder = document.createElement("span");
        placeholder.classList.add("book-cover-placeholder");
        placeholder.textContent = "No Cover";
        cover.appendChild(placeholder);
    }

    // Info
    const info = document.createElement("div");
    info.classList.add("book-info");

    const title = document.createElement("h4");
    title.classList.add("book-title");
    title.textContent = book.title;

    const author = document.createElement("p");
    author.classList.add("book-author");
    author.textContent = book.author;

    info.appendChild(title);
    info.appendChild(author);

    card.appendChild(cover);
    card.appendChild(info);

    return card;
}

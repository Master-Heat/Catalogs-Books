/* ============================================================
   CHARTS PAGE — charts.js
   Handles: sort dropdown, rendering most read / top rated
   ============================================================ */

// ── MOCK DATA ─────────────────────────────────────────────────
// Replace with real API calls when backend is ready
const CHARTS_DATA = {
    mostRead: [
        { id: 1,  title: "To Kill a Mockingbird",           author: "Harper Lee",              cover: null, readers: 45300 },
        { id: 2,  title: "1984",                            author: "George Orwell",            cover: null, readers: 42100 },
        { id: 3,  title: "Pride and Prejudice",             author: "Jane Austen",              cover: null, readers: 38900 },
        { id: 4,  title: "The Great Gatsby",                author: "F. Scott Fitzgerald",      cover: null, readers: 35700 },
        { id: 5,  title: "The Catcher in the Rye",          author: "J.D. Salinger",            cover: null, readers: 33200 },
        { id: 6,  title: "The Hobbit",                      author: "J.R.R. Tolkien",           cover: null, readers: 31600 },
        { id: 7,  title: "Harry Potter and the Sorcerer's Stone", author: "J.K. Rowling",      cover: null, readers: 29900 },
        { id: 8,  title: "The Lord of the Rings",           author: "J.R.R. Tolkien",           cover: null, readers: 28400 },
        { id: 9,  title: "Animal Farm",                     author: "George Orwell",            cover: null, readers: 26100 },
        { id: 10, title: "Brave New World",                 author: "Aldous Huxley",            cover: null, readers: 24900 },
    ],

    topRated: [
        { id: 14, title: "The Brothers Karamazov",          author: "Fyodor Dostoevsky",        cover: null, rating: 4.9 },
        { id: 13, title: "Crime and Punishment",            author: "Fyodor Dostoevsky",        cover: null, rating: 4.8 },
        { id: 11, title: "One Hundred Years of Solitude",   author: "Gabriel García Márquez",   cover: null, rating: 4.8 },
        { id: 16, title: "Anna Karenina",                   author: "Leo Tolstoy",              cover: null, rating: 4.7 },
        { id: 15, title: "War and Peace",                   author: "Leo Tolstoy",              cover: null, rating: 4.7 },
        { id: 12, title: "The Count of Monte Cristo",       author: "Alexandre Dumas",          cover: null, rating: 4.6 },
        { id: 18, title: "Don Quixote",                     author: "Miguel de Cervantes",      cover: null, rating: 4.6 },
        { id: 5,  title: "Moby-Dick",                       author: "Herman Melville",          cover: null, rating: 4.5 },
        { id: 3,  title: "Pride and Prejudice",             author: "Jane Austen",              cover: null, rating: 4.5 },
        { id: 1,  title: "To Kill a Mockingbird",           author: "Harper Lee",              cover: null, rating: 4.4 },
    ],
};

// ── STATE ─────────────────────────────────────────────────────
let currentSort = "most-read"; // default

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", () => {
    /* ── Swap for real API call later ──
       Promise.all([
           fetch("/api/charts/most-read").then(r => r.json()),
           fetch("/api/charts/top-rated").then(r => r.json())
       ]).then(([mostRead, topRated]) => {
           CHARTS_DATA.mostRead = mostRead;
           CHARTS_DATA.topRated = topRated;
           renderCharts();
       });
    */

    initSortDropdown();
    renderCharts();
});

// ── SORT DROPDOWN ─────────────────────────────────────────────
function initSortDropdown() {
    const btn      = document.getElementById("sortBtn");
    const dropdown = document.getElementById("sortDropdown");
    const options  = document.querySelectorAll(".sort-option");
    const label    = document.getElementById("sortLabel");

    // Toggle open/close
    btn.addEventListener("click", (e) => {
        e.stopPropagation();
        dropdown.classList.toggle("open");
    });

    // Select option
    options.forEach(option => {
        option.addEventListener("click", () => {
            const value = option.dataset.value;

            // Update active state
            options.forEach(o => o.classList.remove("active"));
            option.classList.add("active");

            // Update button label
            label.textContent = option.textContent;

            // Update sort and re-render
            currentSort = value;
            renderCharts();

            // Close dropdown
            dropdown.classList.remove("open");
        });
    });

    // Close when clicking outside
    document.addEventListener("click", (e) => {
        if (!document.getElementById("sortWrapper").contains(e.target)) {
            dropdown.classList.remove("open");
        }
    });

    // Set default active
    options.forEach(o => {
        if (o.dataset.value === currentSort) o.classList.add("active");
    });
}

// ── RENDER CHARTS ─────────────────────────────────────────────
function renderCharts() {
    const list = document.getElementById("chartsList");
    list.innerHTML = "";

    const data = currentSort === "most-read"
        ? CHARTS_DATA.mostRead
        : CHARTS_DATA.topRated;

    data.forEach((book, index) => {
        const item = createChartItem(book, index + 1, currentSort);
        list.appendChild(item);
    });
}

// ── CREATE CHART ITEM ─────────────────────────────────────────
function createChartItem(book, rank, sortType) {
    const item = document.createElement("div");
    item.className = "chart-item";

    // Navigate to book page on click
    item.addEventListener("click", () => {
        window.location.href = `bookpage.html?id=${book.id}`;
    });

    // Cover
    const coverHTML = book.cover
        ? `<img src="${book.cover}" alt="${book.title}">`
        : "";

    // Stat — readers count or star rating
    const statHTML = sortType === "most-read"
        ? `<span class="chart-stat">${formatReaders(book.readers)}</span>`
        : buildRatingHTML(book.rating);

    item.innerHTML = `
        <span class="chart-rank">${rank}.</span>
        <div class="chart-cover">${coverHTML}</div>
        <div class="chart-info">
            <div class="chart-book-title">${book.title}</div>
            <div class="chart-book-author">${book.author}</div>
        </div>
        ${statHTML}`;

    return item;
}

// ── HELPERS ───────────────────────────────────────────────────

// Format reader count → "45.3K readers"
function formatReaders(count) {
    if (count >= 1000) {
        return `${(count / 1000).toFixed(1)}K readers`;
    }
    return `${count} readers`;
}

// Build star + rating number HTML
function buildRatingHTML(rating) {
    const fullStars  = Math.floor(rating);
    const hasHalf    = rating % 1 >= 0.5;
    const emptyStars = 5 - fullStars - (hasHalf ? 1 : 0);

    let stars = "";

    for (let i = 0; i < fullStars; i++) {
        stars += `<span class="chart-star">★</span>`;
    }
    if (hasHalf) {
        stars += `<span class="chart-star" style="opacity:0.5;">★</span>`;
    }
    for (let i = 0; i < emptyStars; i++) {
        stars += `<span class="chart-star" style="color:#ddd;">★</span>`;
    }

    return `
        <div class="chart-stat">
            <div class="chart-stars">${stars}</div>
            <span class="chart-rating-value">${rating.toFixed(1)}</span>
        </div>`;
}
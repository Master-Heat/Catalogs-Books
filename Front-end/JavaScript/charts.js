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

// ── HELPERS ───────────────────────────────────────────────────
function getStoredUser() {
    try {
        return JSON.parse(localStorage.getItem("user") || "{}");
    } catch {
        return {};
    }
}

function showAdminPanelIfAdmin() {
    const user = getStoredUser();
    const isAdmin = user.role === "Admin" || user.role === "admin";

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

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", async () => {
    // Try to load real chart data from API
    await loadChartData();
    initSortDropdown();
    renderCharts();
    showAdminPanelIfAdmin();
});

// ── LOAD CHART DATA FROM API ──────────────────────────────────
async function loadChartData() {
    try {
        const token = localStorage.getItem("jwt_token");
        
        // Only try to fetch if user is authenticated
        if (!token) {
            return;
        }

        const headers = {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
        };

        // Try to fetch most-read from popular books endpoint
        try {
            const mostReadUrl = CONFIG.buildUrl(CONFIG.ENDPOINTS.CHARTS_MOST_READ);
            const mostReadRes = await fetch(mostReadUrl, { headers });
            if (mostReadRes.ok) {
                const data = await mostReadRes.json();
                if (Array.isArray(data) && data.length > 0) {
                    CHARTS_DATA.mostRead = data;
                }
            }
        } catch (e) {
            // Silently fall back to mock data
        }

        // Try to fetch top-rated
        try {
            const topRatedUrl = CONFIG.buildUrl(CONFIG.ENDPOINTS.CHARTS_TOP_RATED);
            const topRatedRes = await fetch(topRatedUrl, { headers });
            if (topRatedRes.ok) {
                const data = await topRatedRes.json();
                if (Array.isArray(data) && data.length > 0) {
                    CHARTS_DATA.topRated = data;
                }
            }
        } catch (e) {
            // Silently fall back to mock data
        }

    } catch (error) {
        // Will use mock data as fallback
    }
}

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
        window.location.href = `./bookpage.html?id=${book.id}`;
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
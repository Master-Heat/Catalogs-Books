// ============================================================
// CONFIG
// ============================================================
const APIDomain = "http://localhost:5184";
const MIN_BOOKS_TO_SHOW = 3;

// ============================================================
// API FETCH
// ============================================================

async function getHomepageData(APIDomain, token) {
  const url = `${APIDomain}/api/AccountInfo/homepage`;

  try {
    const response = await fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
        Accept: "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`Error: ${response.status} - ${response.statusText}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Failed to fetch homepage data:", error);
    return null;
  }
}

// ============================================================
// MAP API BOOK → INTERNAL BOOK FORMAT
// ============================================================

function mapApiBook(apiBook) {
  return {
    id: apiBook.bookID,
    title: apiBook.title,
    author: apiBook.author || null,
    cover: apiBook.coverImageLink || null,
    coverAlt: apiBook.coverAlt || apiBook.title,
  };
}

// ============================================================
// RENDER HELPERS
// ============================================================

function createBookCard(book) {
  const card = document.createElement("div");
  card.classList.add("book-card");

  card.style.cursor = "pointer";
  card.addEventListener("click", () => {
    window.location.href = `bookpage.html?id=${book.id}`;
  });

  // Cover
  const cover = document.createElement("div");
  cover.classList.add("book-cover");

  if (book.cover) {
    const img = document.createElement("img");
    img.src = book.cover;
    img.alt = book.coverAlt || book.title;
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
  author.textContent = book.author || "";

  info.appendChild(title);
  info.appendChild(author);

  card.appendChild(cover);
  card.appendChild(info);

  return card;
}

/**
 * Build a single <section> element for a row of books.
 */
function createSection(sectionTitle, books) {
  const section = document.createElement("section");
  section.classList.add("book-section");

  const h2 = document.createElement("h2");
  h2.classList.add("section-title");
  h2.textContent = sectionTitle;

  const scrollWrapper = document.createElement("div");
  scrollWrapper.classList.add("scroll-wrapper");

  const booksRow = document.createElement("div");
  booksRow.classList.add("books-row");

  books.forEach((book) => {
    booksRow.appendChild(createBookCard(book));
  });

  scrollWrapper.appendChild(booksRow);
  section.appendChild(h2);
  section.appendChild(scrollWrapper);

  return section;
}

/**
 * Append a section to the container only if it has >= MIN_BOOKS_TO_SHOW books.
 * All qualifying books are rendered in a single scrollable row.
 */
function renderSection(container, title, apiBooks) {
  if (!apiBooks || apiBooks.length < MIN_BOOKS_TO_SHOW) return;

  const mapped = apiBooks.map(mapApiBook);
  container.appendChild(createSection(title, mapped));
}

// ============================================================
// BUILD THE ENTIRE DASHBOARD
// ============================================================

function buildDashboard(dashboard) {
  const main = document.querySelector("main.main-content");
  if (!main) return;

  // Clear whatever static HTML was there
  main.innerHTML = "";

  // Define the sections we want, in order.
  const sectionDefs = [
    ["Recommended by Category", "categoryRecs"],
    ["Recommended by Author", "authorRecs"],
    ["Recommended for You", "authorAndCategoryRecs"],
    ["Popular This Week", "popularThisWeek"],
    ["Popular All Time", "popularAllTime"],
  ];

  sectionDefs.forEach(([title, key]) => {
    renderSection(main, title, dashboard[key]);
  });

  // If nothing was rendered, show a message
  if (main.children.length === 0) {
    const empty = document.createElement("p");
    empty.style.textAlign = "center";
    empty.style.padding = "2rem";
    empty.textContent = "No recommendations available yet. Start reading to get personalised suggestions!";
    main.appendChild(empty);
  }
}

// ============================================================
// INIT
// ============================================================

document.addEventListener("DOMContentLoaded", async () => {
  const token = localStorage.getItem("jwt_token");

  if (!token) {
    console.warn("No JWT token found – showing empty dashboard.");
    return;
  }

  try {
    const dashboard = await getHomepageData(APIDomain, token);

    if (!dashboard) {
      console.warn("API returned no data.");
      return;
    }

    console.log("Dashboard data:", dashboard);
    buildDashboard(dashboard);
  } catch (err) {
    console.error("Failed to initialise dashboard:", err);
  }
}); 
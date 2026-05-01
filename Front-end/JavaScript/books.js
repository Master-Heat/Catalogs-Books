// ============================================================
// BOOK DATA
// ============================================================

const bookData = {
  recent: [
    { id: 1,  title: "To Kill a Mockingbird", author: "Harper Lee",            cover: "../assets/To_Kill_a_Mockingbird_(first_edition_cover).jpg" },
    { id: 2,  title: "1984",                  author: "George Orwell",          cover: null },
    { id: 3,  title: "Pride and Prejudice",   author: "Jane Austen",            cover: null },
    { id: 4,  title: "The Great Gatsby",      author: "F. Scott Fitzgerald",    cover: null },
    { id: 5,  title: "Moby Dick",             author: "Herman Melville",        cover: null },
    { id: 6,  title: "The Catcher in the Rye",author: "J.D. Salinger",         cover: null },
  ],

  recommended: [
    { id: 7,  title: "Brave New World",        author: "Aldous Huxley",         cover: null },
    { id: 8,  title: "The Hobbit",             author: "J.R.R. Tolkien",        cover: null },
    { id: 9,  title: "Fahrenheit 451",         author: "Ray Bradbury",          cover: null },
    { id: 10, title: "Jane Eyre",              author: "Charlotte Brontë",      cover: null },
    { id: 11, title: "Wuthering Heights",      author: "Emily Brontë",          cover: null },
    { id: 12, title: "The Old Man and the Sea",author: "Ernest Hemingway",      cover: null },
  ],

  popular: [
    { id: 13, title: "Crime and Punishment",    author: "Fyodor Dostoevsky",    cover: null },
    { id: 14, title: "The Brothers Karamazov",  author: "Fyodor Dostoevsky",    cover: null },
    { id: 15, title: "War and Peace",           author: "Leo Tolstoy",          cover: null },
    { id: 16, title: "Anna Karenina",           author: "Leo Tolstoy",          cover: null },
    { id: 17, title: "The Iliad",               author: "Homer",                cover: null },
    { id: 18, title: "Don Quixote",             author: "Miguel de Cervantes",  cover: null },
  ],

  new: [
    { id: 19, title: "Ulysses",          author: "James Joyce",           cover: null },
    { id: 20, title: "The Divine Comedy",author: "Dante Alighieri",       cover: null },
    { id: 21, title: "Hamlet",           author: "William Shakespeare",   cover: null },
    { id: 22, title: "Macbeth",          author: "William Shakespeare",   cover: null },
    { id: 23, title: "Romeo and Juliet", author: "William Shakespeare",   cover: null },
    { id: 24, title: "The Tales of Genji",author: "Murasaki Shikibu",    cover: null },
  ],
};

// ============================================================
// RENDER FUNCTION
// ============================================================

function createBookCard(book) {
  const card = document.createElement("div");
  card.classList.add("book-card");

  // ── Make card clickable ──────────────────────────────────
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
    img.alt = book.title;
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

function renderRow(rowId, books) {
  const row = document.getElementById(rowId);
  if (!row) return;
  books.forEach((book) => {
    const card = createBookCard(book);
    row.appendChild(card);
  });
}

// ============================================================
// INIT
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  renderRow("row-recent",      bookData.recent);
  renderRow("row-recommended", bookData.recommended);
  renderRow("row-popular",     bookData.popular);
  renderRow("row-new",         bookData.new);
});
// ============================================================
// BOOK DATA
// ============================================================

const bookData = {
  recent: [
    { title: "To Kill a Mockingbird", author: "Harper Lee", cover: null },
    { title: "1984", author: "George Orwell", cover: null },
    { title: "Pride and Prejudice", author: "Jane Austen", cover: null },
    { title: "The Great Gatsby", author: "F. Scott Fitzgerald", cover: null },
    { title: "Moby Dick", author: "Herman Melville", cover: null },
    { title: "The Catcher in the Rye", author: "J.D. Salinger", cover: null },
  ],

  recommended: [
    { title: "Brave New World", author: "Aldous Huxley", cover: null },
    { title: "The Hobbit", author: "J.R.R. Tolkien", cover: null },
    { title: "Fahrenheit 451", author: "Ray Bradbury", cover: null },
    { title: "Jane Eyre", author: "Charlotte Brontë", cover: null },
    { title: "Wuthering Heights", author: "Emily Brontë", cover: null },
    { title: "The Old Man and the Sea", author: "Ernest Hemingway", cover: null },
  ],

  popular: [
    { title: "Crime and Punishment", author: "Fyodor Dostoevsky", cover: null },
    { title: "The Brothers Karamazov", author: "Fyodor Dostoevsky", cover: null },
    { title: "War and Peace", author: "Leo Tolstoy", cover: null },
    { title: "Anna Karenina", author: "Leo Tolstoy", cover: null },
    { title: "The Iliad", author: "Homer", cover: null },
    { title: "Don Quixote", author: "Miguel de Cervantes", cover: null },
  ],

  new: [
    { title: "Ulysses", author: "James Joyce", cover: null },
    { title: "The Divine Comedy", author: "Dante Alighieri", cover: null },
    { title: "Hamlet", author: "William Shakespeare", cover: null },
    { title: "Macbeth", author: "William Shakespeare", cover: null },
    { title: "Romeo and Juliet", author: "William Shakespeare", cover: null },
    { title: "The Tales of Genji", author: "Murasaki Shikibu", cover: null },
  ],
};

// ============================================================
// RENDER FUNCTION
// ============================================================

function createBookCard(book) {
  const card = document.createElement("div");
  card.classList.add("book-card");

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
  renderRow("row-recent", bookData.recent);
  renderRow("row-recommended", bookData.recommended);
  renderRow("row-popular", bookData.popular);
  renderRow("row-new", bookData.new);
});
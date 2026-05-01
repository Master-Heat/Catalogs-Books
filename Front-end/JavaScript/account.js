/* ============================================================
   ACCOUNT PAGE — account.js
   Handles: update name, update password, add book (admin)
   ============================================================ */

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", () => {
    loadUserInfo();
    initInfoUpdate();
    initPasswordUpdate();

    // Admin-only
    const addBookBtn = document.getElementById("addBookBtn");
    if (addBookBtn) {
        initAddBook();
    }
});

// ── LOAD USER INFO ────────────────────────────────────────────
function loadUserInfo() {
    /* ── Replace with real API call ──
       fetch("/api/user/me")
         .then(r => r.json())
         .then(user => {
             document.getElementById("userName").value = user.name;
         });
    */
    try {
        const user = JSON.parse(localStorage.getItem("user") || "{}");
        const nameEl = document.getElementById("userName");
        if (nameEl && user.name) nameEl.value = user.name;
    } catch {
        // silent
    }
}

// ── UPDATE INFO ───────────────────────────────────────────────
function initInfoUpdate() {
    const btn = document.getElementById("updateInfo");
    if (!btn) return;

    btn.addEventListener("click", () => {
        const name = document.getElementById("userName").value.trim();

        if (!name) {
            showMessage("Please enter your name.", "error");
            return;
        }

        /* ── Replace with real API call ──
           fetch("/api/user/me", {
               method: "PATCH",
               headers: { "Content-Type": "application/json" },
               body: JSON.stringify({ name })
           }).then(() => showMessage("Name updated successfully.", "success"));
        */

        // Update localStorage for now
        try {
            const user = JSON.parse(localStorage.getItem("user") || "{}");
            user.name = name;
            localStorage.setItem("user", JSON.stringify(user));
        } catch { /* silent */ }

        showMessage("Name updated successfully.", "success");
    });
}

// ── UPDATE PASSWORD ───────────────────────────────────────────
function initPasswordUpdate() {
    const btn = document.getElementById("updatePassword");
    if (!btn) return;

    btn.addEventListener("click", () => {
        const current = document.getElementById("currentPassword").value;
        const next    = document.getElementById("newPassword").value;
        const confirm = document.getElementById("confirmPassword").value;

        if (!current || !next || !confirm) {
            showMessage("Please fill in all password fields.", "error");
            return;
        }

        if (next !== confirm) {
            showMessage("New passwords do not match.", "error");
            return;
        }

        if (next.length < 8) {
            showMessage("New password must be at least 8 characters.", "error");
            return;
        }

        /* ── Replace with real API call ──
           fetch("/api/user/password", {
               method: "PATCH",
               headers: { "Content-Type": "application/json" },
               body: JSON.stringify({ currentPassword: current, newPassword: next })
           })
           .then(r => {
               if (!r.ok) throw new Error("Wrong current password.");
               showMessage("Password updated successfully.", "success");
               document.getElementById("currentPassword").value = "";
               document.getElementById("newPassword").value     = "";
               document.getElementById("confirmPassword").value = "";
           })
           .catch(err => showMessage(err.message, "error"));
        */

        showMessage("Password updated successfully.", "success");
        document.getElementById("currentPassword").value = "";
        document.getElementById("newPassword").value     = "";
        document.getElementById("confirmPassword").value = "";
    });
}

// ── ADD BOOK (admin) ──────────────────────────────────────────
function initAddBook() {
    const addBookBtn  = document.getElementById("addBookBtn");
    const panel       = document.getElementById("addBookPanel");
    const overlay     = document.getElementById("panelOverlay");
    const cancelBtn   = document.getElementById("cancelBook");
    const submitBtn   = document.getElementById("submitBook");
    const coverUpload = document.getElementById("coverUpload");
    const coverInput  = document.getElementById("coverInput");
    const coverPreview= document.getElementById("coverPreview");
    const coverText   = document.getElementById("coverUploadText");

    // Open panel
    addBookBtn.addEventListener("click", () => {
        overlay.style.display = "block";
        panel.classList.add("open");
        document.body.style.overflow = "hidden";
    });

    // Close panel
    function closePanel() {
        panel.classList.remove("open");
        overlay.style.display = "none";
        document.body.style.overflow = "";
        resetBookForm();
    }

    cancelBtn.addEventListener("click", closePanel);
    overlay.addEventListener("click", closePanel);

    // Cover image upload
    coverUpload.addEventListener("click", () => coverInput.click());

    coverInput.addEventListener("change", () => {
        const file = coverInput.files[0];
        if (!file) return;

        const url = URL.createObjectURL(file);
        coverPreview.src         = url;
        coverPreview.style.display = "block";
        coverText.style.display    = "none";
    });

    // Submit book
    submitBtn.addEventListener("click", () => {
        const book = gatherBookForm();
        if (!book) return;

        /* ── Replace with real API call ──
           const formData = new FormData();
           Object.entries(book).forEach(([k, v]) => formData.append(k, v));
           if (coverInput.files[0]) formData.append("cover", coverInput.files[0]);

           fetch("/api/books", { method: "POST", body: formData })
             .then(r => r.json())
             .then(() => {
                 showMessage("Book added successfully.", "success");
                 closePanel();
             })
             .catch(() => showMessage("Failed to add book.", "error"));
        */

        console.log("New book:", book);
        showMessage("Book added successfully.", "success");
        closePanel();
    });
}

// ── GATHER FORM DATA ──────────────────────────────────────────
function gatherBookForm() {
    const fields = {
        title:       document.getElementById("bookTitle").value.trim(),
        author:      document.getElementById("bookAuthor").value.trim(),
        description: document.getElementById("bookDescription").value.trim(),
        year:        document.getElementById("bookYear").value.trim(),
        publisher:   document.getElementById("bookPublisher").value.trim(),
        pages:       document.getElementById("bookPages").value.trim(),
        language:    document.getElementById("bookLanguage").value.trim(),
        isbn:        document.getElementById("bookISBN").value.trim(),
        genre:       document.getElementById("bookGenre").value.trim(),
    };

    // Check required fields
    const missing = Object.entries(fields)
        .filter(([, v]) => !v)
        .map(([k]) => k);

    if (missing.length > 0) {
        showMessage(`Please fill in: ${missing.join(", ")}.`, "error");
        return null;
    }

    return fields;
}

// ── RESET FORM ────────────────────────────────────────────────
function resetBookForm() {
    const ids = [
        "bookTitle", "bookAuthor", "bookDescription",
        "bookYear", "bookPublisher", "bookPages",
        "bookLanguage", "bookISBN", "bookGenre"
    ];
    ids.forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = "";
    });

    const coverInput   = document.getElementById("coverInput");
    const coverPreview = document.getElementById("coverPreview");
    const coverText    = document.getElementById("coverUploadText");

    if (coverInput)   coverInput.value            = "";
    if (coverPreview) coverPreview.style.display  = "none";
    if (coverText)    coverText.style.display     = "inline";
}

// ── SHOW MESSAGE ──────────────────────────────────────────────
function showMessage(text, type = "success") {
    // Remove any existing message
    const existing = document.getElementById("accountMessage");
    if (existing) existing.remove();

    const msg = document.createElement("div");
    msg.id = "accountMessage";
    msg.textContent = text;

    Object.assign(msg.style, {
        position:     "fixed",
        bottom:       "30px",
        left:         "50%",
        transform:    "translateX(-50%)",
        background:   type === "success" ? "#1a1a1a" : "#c0392b",
        color:        "#fff",
        padding:      "12px 24px",
        borderRadius: "4px",
        fontSize:     "13px",
        fontFamily:   "'Product Sans', 'Nunito', sans-serif",
        zIndex:       "9999",
        boxShadow:    "0 4px 12px rgba(0,0,0,0.2)",
        transition:   "opacity 0.3s",
    });

    document.body.appendChild(msg);

    // Auto-remove after 3 seconds
    setTimeout(() => {
        msg.style.opacity = "0";
        setTimeout(() => msg.remove(), 300);
    }, 3000);
}
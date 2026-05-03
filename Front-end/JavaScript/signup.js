/* SIGNUP — signup.js
   Connects to: POST /api/Register/register */

async function performRegister(domain, username, email, password) {
    const url = `${domain}${CONFIG.ENDPOINTS.REGISTER}`;

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, email, password })
        });

        const data = await response.json();

        if (!response.ok) {
            // Backend sends { message: "..." } on error
            throw new Error(data.message || "Registration failed.");
        }

        return data; // { message: "Registration successful. Welcome, username!" }

    } catch (error) {
        return error instanceof Error ? error : new Error("Unexpected error.");
    }
}

// ── Form Submit ───────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", () => {
    const signupForm = document.querySelector(".signup-form");
    if (!signupForm) return;

    signupForm.addEventListener('submit', async (event) => {
        event.preventDefault();

        // Clear old messages
        const existing = signupForm.querySelector(".form-message");
        if (existing) existing.remove();

        const username = document.getElementById('username').value.trim();
        const email    = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value;
        const confirm  = document.getElementById('confirm-password').value;

        // ── Client-side validation ────────────────────────────
        if (!username || !email || !password || !confirm) {
            showFormMessage(signupForm, "Please fill in all fields.", "error");
            return;
        }

        // Validate email format
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showFormMessage(signupForm, "Please enter a valid email address.", "error");
            return;
        }

        if (password !== confirm) {
            showFormMessage(signupForm, "Passwords do not match.", "error");
            return;
        }

        if (password.length < 8) {
            showFormMessage(signupForm, 
                "Password must be at least 8 characters.", "error");
            return;
        }

        // ── Disable button while loading ──────────────────────
        const submitBtn = signupForm.querySelector("button[type='submit']");
        submitBtn.disabled    = true;
        submitBtn.textContent = "Creating Account...";

        const result = await performRegister(CONFIG.API_DOMAIN, username, email, password);

        submitBtn.disabled    = false;
        submitBtn.textContent = "Create Account";

        if (result instanceof Error) {
            showFormMessage(signupForm, result.message, "error");
            return;
        }

        // ✅ Success — show message then redirect to login
        showFormMessage(signupForm, 
            "Account created! Redirecting to login...", "success");

        setTimeout(() => {
            window.location.href = './login.html';
        }, 2000);
    });
});

// ── Helper ────────────────────────────────────────────────────
function showFormMessage(form, text, type) {
    const existing = form.querySelector(".form-message");
    if (existing) existing.remove();

    const msg = document.createElement("p");
    msg.classList.add("form-message");
    msg.textContent   = text;
    msg.style.color   = type === "error" ? "red" : "green";
    msg.style.fontSize = "14px";
    msg.style.marginTop = "8px";
    form.appendChild(msg);
}
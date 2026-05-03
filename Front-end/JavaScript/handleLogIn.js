/* ============================================================
   HANDLE LOGIN — handleLogIn.js
   Connects to: POST /api/LogIn/login
   ============================================================ */

async function performLogin(domain, email, password) {
    const url = `${domain}${CONFIG.ENDPOINTS.LOGIN}`;
    const loginData = { email, password };

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(loginData)
        });

        if (response.status === 401) {
            const message = await response.text();
            throw new Error(message);
        }

        if (!response.ok) {
            throw new Error("An unexpected error occurred.");
        }

        return await response.text(); // Returns raw JWT string

    } catch (error) {
        return error;
    }
}

// ── Helper: decode JWT payload ────────────────────────────────
function parseJwt(token) {
    try {
        const base64 = token.split('.')[1]
            .replace(/-/g, '+')
            .replace(/_/g, '/');
        const json = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(json);
    } catch {
        return null;
    }
}

// ── Form Submit ───────────────────────────────────────────────
const loginForm = document.querySelector(".signup-form");

loginForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    // Remove existing error messages
    const existingError = loginForm.querySelector(".error-message");
    if (existingError) existingError.remove();

    const email    = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;

    // Client-side validation
    if (!email || !password) {
        const errorMsg = document.createElement("p");
        errorMsg.classList.add("error-message");
        errorMsg.style.color    = "red";
        errorMsg.style.fontSize = "15px";
        errorMsg.textContent    = "Please enter both email and password.";
        loginForm.appendChild(errorMsg);
        return;
    }

    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        const errorMsg = document.createElement("p");
        errorMsg.classList.add("error-message");
        errorMsg.style.color    = "red";
        errorMsg.style.fontSize = "15px";
        errorMsg.textContent    = "Please enter a valid email address.";
        loginForm.appendChild(errorMsg);
        return;
    }

    const result = await performLogin(CONFIG.API_DOMAIN, email, password);

    if (result instanceof Error) {
        // Show error on page
        const errorMsg = document.createElement("p");
        errorMsg.classList.add("error-message");
        errorMsg.style.color    = "red";
        errorMsg.style.fontSize = "15px";
        errorMsg.textContent    = result.message;
        loginForm.appendChild(errorMsg);
        return; // Stop here — don't redirect
    }

    // ✅ Save token BEFORE redirecting
    localStorage.setItem('jwt_token', result);

    // ✅ Decode JWT to get user info and save it
    const payload = parseJwt(result);
    if (payload) {
        // Extract role from token claims
        const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
                  ?? payload["role"]
                  ?? "User";

        const userId = payload["sub"]
                    ?? payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
                    ?? null;

        const userName = payload["name"]
                      ?? payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]
                      ?? "";

        // Save user info for use across pages
        localStorage.setItem('user', JSON.stringify({
            id:    userId,
            name:  userName,
            email: email,
            role:  role
        }));
    }

    // ✅ Redirect AFTER saving
    window.location.href = './homepage.html';
});
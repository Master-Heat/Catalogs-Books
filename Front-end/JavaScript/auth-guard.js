/* ============================================================
   AUTH GUARD — auth-guard.js
   Protects routes and manages authentication state
   ============================================================ */

const AuthGuard = (() => {
    // ── STATE ─────────────────────────────────────────────────────
    const PUBLIC_PAGES = ['index.html', 'login.html', 'signup.html'];
    const PROTECTED_PAGES = ['homepage.html', 'bookpage.html', 'useraccount.html', 'charts.html', 'aichat.html', 'adminaccount.html'];
    const AUTH_PAGES = ['login.html', 'signup.html'];

    // ── GET CURRENT PAGE ──────────────────────────────────────────
    function getCurrentPage() {
        const path = window.location.pathname;
        return path.substring(path.lastIndexOf('/') + 1) || 'index.html';
    }

    // ── CHECK IF USER IS LOGGED IN ────────────────────────────────
    function isLoggedIn() {
        const token = localStorage.getItem('jwt_token');
        return !!token && token.trim() !== '';
    }

    // ── GET USER INFO ─────────────────────────────────────────────
    function getUserInfo() {
        try {
            return JSON.parse(localStorage.getItem('user') || '{}');
        } catch {
            return {};
        }
    }

    // ── CHECK IF USER IS ADMIN ────────────────────────────────────
    function isAdmin() {
        const user = getUserInfo();
        return user.role === "Admin" || user.role === "admin";
    }

    // ── PROTECT ROUTES ────────────────────────────────────────────
    function protectRoutes() {
        const currentPage = getCurrentPage();
        const loggedIn = isLoggedIn();
        const user = getUserInfo();

        // If user is NOT logged in
        if (!loggedIn) {
            // Only allow public pages
            if (!PUBLIC_PAGES.includes(currentPage)) {
                window.location.href = './login.html';
                return;
            }
        } 
        // If user IS logged in
        else {
            // Don't allow access to auth pages (login, signup)
            if (AUTH_PAGES.includes(currentPage)) {
                window.location.href = './homepage.html';
                return;
            }

            // Only admin can access adminaccount.html
            if (currentPage === 'adminaccount.html' && !isAdmin()) {
                window.location.href = './homepage.html';
                return;
            }
        }
    }

    // ── LOGOUT ────────────────────────────────────────────────────
    function logout() {
        localStorage.removeItem('jwt_token');
        localStorage.removeItem('user');
        window.location.href = './login.html';
    }

    // ── PUBLIC API ────────────────────────────────────────────────
    return {
        protectRoutes,
        logout,
        isLoggedIn,
        getUserInfo,
        isAdmin,
        getCurrentPage
    };
})();

// ── RUN ON PAGE LOAD ──────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    AuthGuard.protectRoutes();
});

// ── ALSO RUN BEFORE PAGE UNLOAD (for edge cases) ────────────
window.addEventListener('pageshow', () => {
    AuthGuard.protectRoutes();
});

/* ============================================================
   CONFIG — config.js
   Centralized configuration for frontend
   ============================================================ */

const CONFIG = {
    // API Domain - change this one place to update everywhere
    API_DOMAIN: "http://localhost:5184",
    
    // API Endpoints
    ENDPOINTS: {
        // Auth
        LOGIN:   "/api/LogIn/login",
        REGISTER: "/api/Register/register",
        
        // Account
        ACCOUNT_INFO: "/api/AccountInfo",
        ACCOUNT_HOMEPAGE: "/api/AccountInfo/homepage",
        
        // Books
        BOOKS: "/api/Books",
        BOOKS_AUTHORIZED: "/api/Books/Authorized",
        BOOKS_SEARCH: "/api/Books/Authorized/search",
        
        // Reviews
        REVIEWS: "/api/Reviews",
        REVIEWS_SUBMIT: "/api/Reviews/submit",
        
        // Lists
        BOOK_LISTS: "/api/BookLists",
        BOOK_LISTS_AUTHORIZED: "/api/BookLists/Authorized",
        
        // Charts
        CHARTS_MOST_READ: "/api/charts/most-read",
        CHARTS_TOP_RATED: "/api/charts/top-rated",
    },
    
    // Helper: Build full API URL
    buildUrl: function(endpoint) {
        return this.API_DOMAIN + endpoint;
    }
};

// Make CONFIG globally available
if (typeof window !== 'undefined') {
    window.CONFIG = CONFIG;
}

/* ============================================================
   UTILITY FUNCTIONS — Shared helpers
   ============================================================ */

/**
 * Escapes HTML special characters to prevent XSS attacks
 * @param {string} text - Text to escape
 * @returns {string} - Escaped text safe for HTML
 */
function escapeHTML(text) {
    if (!text) return '';
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#39;'
    };
    return String(text).replace(/[&<>"']/g, char => map[char]);
}

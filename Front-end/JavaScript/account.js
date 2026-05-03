/* ============================================================
   ACCOUNT PAGE — account.js
   Connects to: GET /api/AccountInfo/{id}
   ============================================================ */

// ── HELPERS ───────────────────────────────────────────────────
function getAuthHeaders() {
    const token = localStorage.getItem("jwt_token");
    return {
        "Authorization": `Bearer ${token}`,
        "Content-Type":  "application/json"
    };
}

function getStoredUser() {
    try {
        return JSON.parse(localStorage.getItem("user") || "{}");
    } catch {
        return {};
    }
}

// ── SHOW ADMIN PANEL FOR ADMINS ───────────────────────────────
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
    const token = localStorage.getItem("jwt_token");

    if (!token) {
        window.location.href = "./login.html";
        return;
    }

    await loadUserInfo();
    initInfoUpdate();
    initPasswordUpdate();
    initLogout();
    showAdminPanelIfAdmin();
});

// ── LOAD USER INFO ────────────────────────────────────────────
async function loadUserInfo() {
    const storedUser = getStoredUser();
    const userId     = storedUser.id;

    if (!userId) {
        showMessage("Could not identify user. Please log in again.", "error");
        return;
    }

    try {
        const response = await fetch(
            `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.ACCOUNT_INFO}/${userId}`,
            { method: "GET", headers: getAuthHeaders() }
        );

        if (response.status === 401) {
            localStorage.removeItem("jwt_token");
            localStorage.removeItem("user");
            window.location.href = "./login.html";
            return;
        }

        if (!response.ok) throw new Error("Failed to load account info.");

        // API returns UserAccountDTO:
        // { accountID, userName, email, role, ... }
        const data = await response.json();

        // Populate fields
        const nameEl  = document.getElementById("userName");
        const emailEl = document.getElementById("userEmail");

        if (nameEl)  nameEl.value  = data.userName || "";
        if (emailEl) emailEl.value = data.email    || "";

        // Update stored user with fresh data
        localStorage.setItem("user", JSON.stringify({
            id:    data.accountID,
            name:  data.userName,
            email: data.email,
            role:  data.role
        }));

    } catch (error) {
        console.error("Failed to load user info:", error);
        showMessage("Failed to load account info.", "error");
    }
}

// ── UPDATE USER INFO ──────────────────────────────────────────
function initInfoUpdate() {
    const btn = document.getElementById("updateInfo");
    if (!btn) return;

    btn.addEventListener("click", async () => {
        const nameEl = document.getElementById("userName");
        const name = nameEl.value.trim();

        if (!name) {
            showMessage("Please enter a name.", "error");
            return;
        }

        if (name.length < 2) {
            showMessage("Name must be at least 2 characters.", "error");
            return;
        }

        try {
            const storedUser = getStoredUser();
            const userId = storedUser.id;

            const response = await fetch(
                `${CONFIG.API_DOMAIN}${CONFIG.ENDPOINTS.ACCOUNT_INFO}/${userId}`,
                {
                    method: "PUT",
                    headers: getAuthHeaders(),
                    body: JSON.stringify({ userName: name })
                }
            );

            if (response.status === 401) {
                localStorage.removeItem("jwt_token");
                localStorage.removeItem("user");
                window.location.href = "./login.html";
                return;
            }

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || "Failed to update user info.");
            }

            const updatedUser = getStoredUser();
            updatedUser.name = name;
            localStorage.setItem("user", JSON.stringify(updatedUser));

            showMessage("User information updated successfully.", "success");
        } catch (error) {
            console.error("Failed to update user info:", error);
            showMessage(error.message || "Failed to update user information.", "error");
        }
    });
}

// ── UPDATE PASSWORD ───────────────────────────────────────────
function initPasswordUpdate() {
    const btn = document.getElementById("updatePassword");
    if (!btn) return;

    btn.addEventListener("click", async () => {
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

        // Note: Wire to password update endpoint when available
        // For now show coming soon message
        showMessage("Password update coming soon.", "success");

        document.getElementById("currentPassword").value = "";
        document.getElementById("newPassword").value     = "";
        document.getElementById("confirmPassword").value = "";
    });
}

// ── LOGOUT ────────────────────────────────────────────────────
function initLogout() {
    const logoutBtn = document.getElementById("logoutBtn");
    if (!logoutBtn) return;

    logoutBtn.addEventListener("click", () => {
        AuthGuard.logout();
    });
}

// ── SHOW MESSAGE ──────────────────────────────────────────────
function showMessage(text, type = "success") {
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
        zIndex:       "9999",
        boxShadow:    "0 4px 12px rgba(0,0,0,0.2)",
    });

    document.body.appendChild(msg);

    setTimeout(() => {
        msg.style.opacity    = "0";
        msg.style.transition = "opacity 0.3s";
        setTimeout(() => msg.remove(), 300);
    }, 3000);
}
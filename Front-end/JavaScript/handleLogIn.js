const APIDomain = "http://localhost:5184";

async function performLogin(domain, email, password) {
    const url = `${domain}/api/LogIn/login`;
    const loginData = { email, password };

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(loginData)
        });

        // 1. Handle 401 Unauthorized (Invalid credentials)
        if (response.status === 401) {
            const message = await response.text(); // Gets "Invalid User Name Or Password"
            throw new Error(message);
        }

        // 2. Handle other non-OK responses
        if (!response.ok) {
            console.error("Server Error Details:", await response.text());
            throw new Error("An unexpected error occurred.");
        }

        // 3. Success
        return await response.json();

    } catch (error) {
        // Rethrow or return the error message to be caught/handled by the UI
        return error; 
    }
}

const loginForm = document.querySelector(".signup-form");

loginForm.addEventListener('submit', async (event) => {
    // event.preventDefault();

    // Remove existing error messages if any
    const existingError = loginForm.querySelector(".error-message");
    if (existingError) existingError.remove();

    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    const result = await performLogin(APIDomain, email, password);

    // Check if the result is an Error object (failure) or a Data object (success)
    if (result instanceof Error) {
        const errorMsg = document.createElement("p");
        errorMsg.classList.add("error-message");
        errorMsg.style.color = "red";
        errorMsg.style.fontSize = "15px";
        errorMsg.textContent = result.message;
        loginForm.appendChild(errorMsg);
    } else {
        // Success Logic
        console.log(result);

        // window.location.href = "/dashboard.html";
    }
    token = result.token
    localStorage.setItem('jwt_token', token);
    
});
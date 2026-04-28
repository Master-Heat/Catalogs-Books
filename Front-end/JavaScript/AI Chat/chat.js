/**
 * Chat UI Controller
 * Handles all DOM interactions and message rendering.
 * No AI logic here — fully separated from api.js
 */

const Chat = (() => {

  // ── DOM refs ──────────────────────────────────────────────
  const messagesContainer = document.getElementById("chat-messages");
  const inputField        = document.getElementById("chat-input");
  const sendButton        = document.getElementById("send-btn");
  const sendBtnText       = document.getElementById("send-btn-text");
  const sendBtnIcon       = document.getElementById("send-btn-icon");

  // ── State ─────────────────────────────────────────────────
  let isLoading = false;

  // ─────────────────────────────────────────────────────────
  //  RENDER HELPERS
  // ─────────────────────────────────────────────────────────

  /**
   * Creates and appends a message bubble to the chat.
   * @param {string} text    - Message content
   * @param {"ai"|"user"} sender
   * @param {boolean} isTyping - Shows animated dots instead of text
   * @returns {HTMLElement} the bubble element
   */
  function _createBubble(text, sender, isTyping = false) {
    const wrapper = document.createElement("div");
    wrapper.classList.add("message-wrapper", `message-wrapper--${sender}`);

    const bubble = document.createElement("div");
    bubble.classList.add("message-bubble", `message-bubble--${sender}`);

    if (isTyping) {
      bubble.classList.add("message-bubble--typing");
      bubble.innerHTML = `
        <span class="typing-dot"></span>
        <span class="typing-dot"></span>
        <span class="typing-dot"></span>
      `;
    } else {
      // Safely set text (prevents XSS)
      bubble.textContent = text;
    }

    wrapper.appendChild(bubble);
    messagesContainer.appendChild(wrapper);

    _scrollToBottom();
    return bubble;
  }

  function _scrollToBottom() {
    messagesContainer.scrollTo({
      top:      messagesContainer.scrollHeight,
      behavior: "smooth",
    });
  }

  // ─────────────────────────────────────────────────────────
  //  LOADING STATE
  // ─────────────────────────────────────────────────────────
  function _setLoading(state) {
    isLoading = state;
    inputField.disabled  = state;
    sendButton.disabled  = state;

    if (state) {
      sendBtnIcon.style.display = "none";
      sendBtnText.textContent   = "...";
    } else {
      sendBtnIcon.style.display = "inline";
      sendBtnText.textContent   = "Send";
      inputField.focus();
    }
  }

  // ─────────────────────────────────────────────────────────
  //  SEND MESSAGE FLOW
  // ─────────────────────────────────────────────────────────
  async function sendMessage() {
    const text = inputField.value.trim();
    if (!text || isLoading) return;

    // 1. Render user bubble
    _createBubble(text, "user");
    inputField.value = "";
    _autoResizeInput();

    // 2. Lock UI + show typing indicator
    _setLoading(true);
    const typingBubble = _createBubble("", "ai", true);

    try {
      // 3. Call AI service (api.js)
      const aiReply = await AIService.sendMessage(text);

      // 4. Replace typing indicator with real response
      typingBubble.classList.remove("message-bubble--typing");
      typingBubble.innerHTML = ""; // clear dots
      typingBubble.textContent = aiReply;

    } catch (err) {
      typingBubble.classList.remove("message-bubble--typing");
      typingBubble.classList.add("message-bubble--error");
      typingBubble.textContent =
        err.message || "Something went wrong. Please try again.";
    } finally {
      // 5. Unlock UI
      _setLoading(false);
      _scrollToBottom();
    }
  }

  // ─────────────────────────────────────────────────────────
  //  INPUT AUTO-RESIZE
  // ─────────────────────────────────────────────────────────
  function _autoResizeInput() {
    inputField.style.height = "auto";
    inputField.style.height = Math.min(inputField.scrollHeight, 120) + "px";
  }

  // ─────────────────────────────────────────────────────────
  //  EVENT LISTENERS
  // ─────────────────────────────────────────────────────────
  function _bindEvents() {
    // Send on button click
    sendButton.addEventListener("click", sendMessage);

    // Send on Enter (Shift+Enter = new line)
    inputField.addEventListener("keydown", (e) => {
      if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        sendMessage();
      }
    });

    // Auto-resize textarea as user types
    inputField.addEventListener("input", _autoResizeInput);
  }

  // ─────────────────────────────────────────────────────────
  //  PUBLIC
  // ─────────────────────────────────────────────────────────
  function init() {
    _bindEvents();
    inputField.focus();
  }

  return { init };
})();
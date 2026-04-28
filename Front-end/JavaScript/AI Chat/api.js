/**
 * ============================================================
 *  API INTEGRATION FILE
 *  ============================================================
 *  @description  This is the ONLY file the AI developer needs
 *                to modify. Replace the mock function below
 *                with the real AI service call.
 *
 *  @contract
 *    - Input  : userMessage (string)
 *    - Output : Promise<string>  →  the AI reply text
 *
 *  @example  Expected usage (already wired, do not change):
 *    const reply = await AIService.sendMessage("Find me a book");
 *    // reply → "I recommend 'Dune' by Frank Herbert..."
 * ============================================================
 */

const AIService = (() => {

  // ─────────────────────────────────────────────────────────
  //  CONFIG  (AI developer fills these in)
  // ─────────────────────────────────────────────────────────
  const CONFIG = {
    API_URL:     "https://your-ai-endpoint.com/api/chat", // ← change this
    API_KEY:     "YOUR_API_KEY_HERE",                     // ← change this
    MODEL:       "gpt-4o",                                // ← change this if needed
    TIMEOUT_MS:  30000,
  };

  // ─────────────────────────────────────────────────────────
  //  CONVERSATION HISTORY  (context sent to AI)
  // ─────────────────────────────────────────────────────────
  const conversationHistory = [
    {
      role: "system",
      content: `You are a helpful AI book assistant. 
                You help users find book recommendations, 
                answer questions about books, and discuss literature.
                Be concise, friendly, and knowledgeable.`,
    },
  ];

  // ─────────────────────────────────────────────────────────
  //  MOCK  (remove this function when connecting real AI)
  // ─────────────────────────────────────────────────────────
  async function _mockResponse(message) {
    await new Promise((r) => setTimeout(r, 1200)); // simulate latency
    return `This is a mock AI response. In a real application, 
this would be connected to an AI service to provide 
personalized book recommendations and answers.`;
  }

  // ─────────────────────────────────────────────────────────
  //  REAL IMPLEMENTATION  (uncomment & use when ready)
  // ─────────────────────────────────────────────────────────
  async function _realResponse(message) {
    conversationHistory.push({ role: "user", content: message });

    const controller = new AbortController();
    const timeout = setTimeout(() => controller.abort(), CONFIG.TIMEOUT_MS);

    try {
      const response = await fetch(CONFIG.API_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${CONFIG.API_KEY}`,
        },
        body: JSON.stringify({
          model:    CONFIG.MODEL,
          messages: conversationHistory,
        }),
        signal: controller.signal,
      });

      if (!response.ok) {
        throw new Error(`API error: ${response.status} ${response.statusText}`);
      }

      const data = await response.json();

      // ── Adjust this path to match your API's response shape ──
      const aiReply = data.choices?.[0]?.message?.content
                   ?? data.reply
                   ?? data.message
                   ?? "Sorry, I couldn't understand the response.";

      conversationHistory.push({ role: "assistant", content: aiReply });

      return aiReply;

    } catch (err) {
      if (err.name === "AbortError") {
        throw new Error("Request timed out. Please try again.");
      }
      throw err;
    } finally {
      clearTimeout(timeout);
    }
  }

  // ─────────────────────────────────────────────────────────
  //  PUBLIC API  — called by the frontend (do not rename)
  // ─────────────────────────────────────────────────────────
  async function sendMessage(userMessage) {
    // ↓ Switch to _realResponse(userMessage) when AI is ready
    return await _mockResponse(userMessage);
  }

  function clearHistory() {
    conversationHistory.splice(1); // keep system prompt
  }

  return { sendMessage, clearHistory };
})();
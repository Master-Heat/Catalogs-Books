/* Search redirect */
function handleSearch(input) {
    input.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            const query = input.value.trim();
            if (query) {
                window.location.href = `./search.html?q=${encodeURIComponent(query)}`;
            }
        }
    });
}
handleSearch(document.getElementById('navSearchInput'));
handleSearch(document.getElementById('mobileSearchInput'));
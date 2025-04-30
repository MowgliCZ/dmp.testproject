document.addEventListener('DOMContentLoaded', function () {
    const input = document.getElementById('fname');
    const results = document.getElementById('search-results');
    let timeout = null;

    input.addEventListener('input', function () {
        const term = input.value.trim();
        results.innerHTML = "";

        if (term.length < 1) return;

        if (timeout) clearTimeout(timeout);
        timeout = setTimeout(() => {
            fetch(`/Feed/SearchFeeds?term=${encodeURIComponent(term)}`)
                .then(response => response.json())
                .then(data => {
                    if (data.length === 0) {
                        results.innerHTML = "<div style='padding: 8px; color: #666;'>No feeds found.</div>";
                        return;
                    }
                    data.forEach(feed => {
                        const div = document.createElement('div');
                        const regex = new RegExp(`(${term})`, 'gi');
                        div.innerHTML = feed.name.replace(regex, '<b>$1</b>');
                        div.style.padding = "8px";
                        div.style.cursor = "pointer";
                        div.style.borderBottom = "1px solid #eee";
                        div.onclick = () => window.location.href = `/Feed/Details/${feed.id}`;
                        results.appendChild(div);
                    });
                });
        }, 300);
    });

    document.addEventListener('click', (e) => {
        if (!input.contains(e.target) && !results.contains(e.target)) {
            results.innerHTML = "";
        }
    });
});
//CARDS ANIMATION
document.addEventListener("DOMContentLoaded", () => {
    const cards = document.querySelectorAll(".card");

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                setTimeout(() => {
                    entry.target.classList.add("visible");
                    entry.target.classList.remove("hidden");
                }, 200);
            } else {
                // Resetuj animaciju kada kartica izađe iz vidnog polja
                entry.target.classList.remove("visible");
                entry.target.classList.add("hidden");
            }
        });
    });

    cards.forEach((card) => observer.observe(card));
});

//MOVING VIEW OF SITE SMOOTHLY
const scrollLinks = document.querySelectorAll(".scroll-link");

scrollLinks.forEach((link) => {
    link.addEventListener("click", function (event) {
        event.preventDefault(); // Sprečava podrazumevanu akciju linka

        const targetId = this.getAttribute("href").substring(1);
        const targetElement = document.getElementById(targetId);

        // Skroluje do sekcije ako postoji
        if (targetElement) {
            targetElement.scrollIntoView({ behavior: "smooth" });
        }
    });
});

//KODIRANJE FAJLA
const fileEncodeInput = document.getElementById("fileEncode");
const checkboxes = document.querySelectorAll(".checkbox");
const encodeBtn = document.getElementById("encode-btn");

// SALJE SERVERU KOJI ALGORITAM JE IZABRAN I OMOGUCAVA BIRANJE
checkboxes.forEach((checkbox) => {
    checkbox.addEventListener("change", async () => {
        // Omogućava samo jedan checkbox da bude selektovan
        checkboxes.forEach((cb) => {
            if (cb !== checkbox) {
                cb.checked = false;
            }
        });

        // Omogućava ili onemogućava unos za fajl
        fileEncodeInput.disabled = !checkbox.checked;

        // Ako je checkbox selektovan, šalje podatke serveru
        if (checkbox.checked) {
            try {
                const response = await fetch("Fsw/Checkbox", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ algorithmType: checkbox.value })  // JSON object
                });



                if (response.ok) {
                    const data = await response.text(); // Menja se sa .json() na .text()
                    console.log(data); // Ispisuje tekstualni odgovor
                } else {
                    console.error("Failed to fetch:", response.statusText);
                }
            } catch (error) {
                console.error("Error:", error);
            }
        }
    });
});

// OMOGUCAVA DUGME DA BUDE STISNUTO I CONSOL LOGUJE KOJI CE FAJL BITI ENKRIPTOVAN
fileEncodeInput.addEventListener("change", () => {
    //ako je dodat neki fajl,omoguci dugme Encode
    encodeBtn.disabled = !fileEncodeInput.files.length;
    console.log("File selected:", fileEncodeInput.files[0]?.name);
});

//SALJE SERVERU FAJL ZA KODIRANJE
encodeBtn.addEventListener("click", async () => {
    if (!fileEncodeInput.files || !fileEncodeInput.files[0]) {
        console.log("No file selected.");
        return;
    }

    const selectedFile = fileEncodeInput.files[0];

    const formData = new FormData();
    formData.append("file", selectedFile);

    try {
        const response = await fetch("Fsw/Upload", {
            method: "POST",
            body: formData
        });

        if (response.ok) {
            const data = await response.json();
            console.log(data.message);
        } else {
            console.error("Error uploading file:", response.statusText);
        }
    } catch (error) {
        console.error("Error:", error);
    }
});

/////////////

//
//
//
//
//
//SLANJE FAJLA

const hostInput = document.getElementById('hostInput');
const portInput = document.getElementById('portInput');
const fileInput = document.getElementById('fileSend');
const sendBtn = document.getElementById('send-btn');

// Enable the send button only when a file is selected
fileInput.addEventListener('change', () => {
    sendBtn.disabled = fileInput.files.length === 0;
});

sendBtn.addEventListener('click', async () => {
    const host = hostInput.value.trim();
    const port = parseInt(portInput.value, 10);
    const file = fileInput.files[0];

    // Basic validation
    if (!host) {
        return alert('Please enter a host.');
    }
    if (!port) {
        return alert('Please enter a valid port.');
    }
    if (!file) {
        return alert('Please select a file.');
    }

    // Build form data
    const formData = new FormData();
    formData.append('host', host);
    formData.append('port', port);
    formData.append('file', file);

    try {
        const response = await fetch('/Tcp/SendFile', {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const result = await response.json();
            console.log('Success:', result);
            alert(result.message);
        } else {
            const errorText = await response.text();
            console.error('Server error:', errorText);
            alert('Error: ' + errorText);
        }
    } catch (err) {
        console.error('Network error:', err);
        alert('Network error occurred. See console for details.');
    }
});

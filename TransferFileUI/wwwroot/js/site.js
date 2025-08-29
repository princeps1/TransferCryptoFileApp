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
        event.preventDefault(); 

        const targetId = this.getAttribute("href").substring(1);
        const targetElement = document.getElementById(targetId);

        
        if (targetElement) {
            targetElement.scrollIntoView({ behavior: "smooth" });
        }
    });
});


// ... (ostali kod ostaje nepromenjen)

// KODIRANJE FAJLA
const fileEncodeInput = document.getElementById("fileEncode");
const checkboxes = document.querySelectorAll(".checkbox");
const encodeBtn = document.getElementById("encode-btn");
const decodeBtn = document.getElementById("decode-btn");

// Dodajemo promenljivu za trenutno izabrani algoritam
let selectedAlgorithm = null;

// SALJE SERVERU KOJI ALGORITAM JE IZABRAN I OMOGUCAVA BIRANJE
checkboxes.forEach((checkbox) => {
    checkbox.addEventListener("change", async () => {
        checkboxes.forEach((cb) => {
            if (cb !== checkbox) {
                cb.checked = false;
            }
        });

        fileEncodeInput.disabled = !checkbox.checked;

        if (checkbox.checked) {
            selectedAlgorithm = checkbox.value; // Pamti izabrani algoritam
            try {
                const response = await fetch("Fsw/Checkbox", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ algorithmType: checkbox.value })
                });

                if (response.ok) {
                    const data = await response.text();
                    console.log(data);
                } else {
                    console.error("Failed to fetch:", response.statusText);
                }
            } catch (error) {
                console.error("Error:", error);
            }
        } else {
            selectedAlgorithm = null; // Resetuj ako je odčekirano
        }
    });
});

// OMOGUCAVA DUGME DA BUDE STISNUTO I CONSOL LOGUJE KOJI FAJL JE SELEKTOVAN
fileEncodeInput.addEventListener("change", () => {
    encodeBtn.disabled = !fileEncodeInput.files.length || !selectedAlgorithm;
    decodeBtn.disabled = !fileEncodeInput.files.length || !selectedAlgorithm;
    console.log("File selected:", fileEncodeInput.files[0]?.name);
});

// SALJE SERVERU FAJL ZA KODIRANJE
encodeBtn.addEventListener("click", async () => {
    if (!fileEncodeInput.files || !fileEncodeInput.files[0]) {
        console.log("No file selected.");
        return;
    }
    if (!selectedAlgorithm) {
        alert("Please select an algorithm first.");
        return;
    }

    const selectedFile = fileEncodeInput.files[0];

    const formData = new FormData();
    formData.append("file", selectedFile);
    formData.append("algorithmType", selectedAlgorithm); // Dodaj algoritam u formu

    try {
        const response = await fetch("Fsw/Upload", {
            method: "POST",
            body: formData
        });

        if (response.ok) {
            const data = await response.json();
            console.log(data.message);
            alert(data.message);
        } else {
            console.error("Error uploading file:", response.statusText);
        }
    } catch (error) {
        console.error("Error:", error);
    }
});

// DEKODIRANJE FAJLA
decodeBtn.addEventListener("click", async () => {
    if (!fileEncodeInput.files || !fileEncodeInput.files[0]) {
        console.log("No file selected.");
        return;
    }
    if (!selectedAlgorithm) {
        alert("Please select an algorithm first.");
        return;
    }

    const selectedFile = fileEncodeInput.files[0];

    const formData = new FormData();
    formData.append("file", selectedFile);
    formData.append("algorithmType", selectedAlgorithm); // Dodaj algoritam u formu

    try {
        const response = await fetch("Fsw/uploadDecrypt", {
            method: "POST",
            body: formData
        });

        if (response.ok) {
            const data = await response.json();
            console.log(data.message);
            alert(data.message);
        } else {
            console.error("Error uploading file:", response.statusText);
        }
    } catch (error) {
        console.error("Error:", error);
    }
});

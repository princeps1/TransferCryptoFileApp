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

checkboxes.forEach((checkbox) => {
  checkbox.addEventListener("change", () => {
    checkboxes.forEach((cb) => {
      if (cb !== checkbox) cb.checked = false;
    });
    fileEncodeInput.disabled = !checkbox.checked;
  });
});

fileEncodeInput.addEventListener("change", () => {
  //ako je dodat neki fajl,omoguci dugme Encode
  encodeBtn.disabled = !fileEncodeInput.files.length;
  console.log("File selected:", fileEncodeInput.files[0]?.name);
});

encodeBtn.addEventListener("click", async () => {
  if (!fileEncodeInput.files || !fileEncodeInput.files[0]) {
    console.log("No file selected.");
    return;
  }

  const selectedFile = fileEncodeInput.files[0];
  const checkboxes = document.querySelectorAll(".checkbox:checked");
  const selectedAlgorithms = Array.from(checkboxes).map((box) => box.value);

  if (selectedAlgorithms.length === 0) {
    console.log("No encoding algorithm selected.");
    return;
  }

  const formData = new FormData();
  formData.append("file", selectedFile);

  try {
    const response = await fetch("https://localhost:7080/File/upload", {
      method: "POST",
      body: formData,
    });

    if (response.ok) {
      const data = await response.json();
      console.log(data.message);

      // Možeš dodatno obraditi odabir algoritama ovde
      console.log("Selected algorithms:", selectedAlgorithms);
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
const fileSendInput = document.getElementById("fileSend");
const sendBtn = document.getElementById("send-btn");

fileSendInput.addEventListener("change", () => {
  sendBtn.disabled = !fileSendInput.files.length;
});

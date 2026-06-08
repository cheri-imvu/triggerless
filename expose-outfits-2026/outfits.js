let outfits = [];

const input = document.getElementById('input');
const pasteBtn = document.getElementById('paste');
const clearBtn = document.getElementById("clear");

// Handle manual paste (Ctrl+V)
input.addEventListener("paste", () => {
  // Wait for paste to complete
  setTimeout(() => {
    processText();
  }, 0)
});

// Handle button click paste
pasteBtn.addEventListener('click', async () => {
  try {
    const text = await navigator.clipboard.readText();
    input.value = text;
    await processText();
  } catch (err) {
    console.error("Clipboard read failed:", err);
  }
});

clearBtn.addEventListener('click', () => {
  input.value = '';
  const container = document.getElementById("outfits-container");
  container.replaceChildren();
});

async function processText() {
  const loading = document.getElementById("loading");
  loading.style.display = "block";
  outfits = [];
  await parseInput();
  generateOutfits();
  loading.style.display = "none";
}

async function parseInput()
{
  let raw = input.value;
  let query = raw.split('?', 2)[1];
  let params = query.split('&');
  for (let i = 0; i < params.length; i++)
  {
    var newOutfit = null;
    let pair = params[i].split('=');
    let name = pair[0];
    let value = pair[1];
    
    if (name === 'room') {
      let rawPids = value.split('%3B');
      let pids = [];
      for (var rawPid of rawPids) {
        pids.push(rawPid.split('x')[0]);
      }

      newOutfit = {
        id: 0,
        display: 'Room',
        pids: pids,
        products: []
      };
    }
    else if (name.indexOf('avatar') === 0) {
      var cid = new Number(name.substr(6));
      const data = await getData(cid, 'user');
      if (data === null) continue;
      newOutfit = {
        id: cid,
        display: data.username.replace('Guest_', ''),
        pids: value.split('%3B'),
        products: []
      };
    }
    else if (name === 'login_id') {
      await sendOutfitRequest(value, raw);
      // send login_id to the server
      // sendToServer(value)

    }

    if (newOutfit != null) outfits.push(newOutfit)
    outfits.sort((a, b) => {
      // Rule 1: id === 0 goes first
      if (a.id === 0 && b.id !== 0) return -1;
      if (b.id === 0 && a.id !== 0) return 1;

      // Rule 2: alphabetical by display
      return a.display.localeCompare(b.display);
    })
  }
}

function generateOutfits() {
  const container = document.getElementById("outfits-container");
  container.replaceChildren();

  for (const outfit of outfits) {
    const div = document.createElement('div');
    div.className = 'outfit';

    const header = document.createElement("div");
    header.className = "outfit-header";
    div.appendChild(header);

    const expander = document.createElement('span');
    expander.textContent = "+";
    expander.className = "expander";
    expander.id = `exp_${outfit.id}`;
    expander.addEventListener("click", async () => onExpand(outfit));
    header.appendChild(expander);

    const display = document.createElement('span');
    display.className = "display";
    display.textContent = `${outfit.display} (${outfit.pids.length})`;
    header.appendChild(display);

    const products = document.createElement('div');
    products.id = `products_${outfit.id}`;
    products.className = 'products';
    products.style.display = 'none';
    div.appendChild(products);

    container.appendChild(div);
  }
}

async function onExpand(outfit)
{
  const DEFAULT_IMAGE = "https://www.triggerless.com/i/dummy.png";

  const expander = document.getElementById(`exp_${outfit.id}`);
  const products = document.getElementById(`products_${outfit.id}`);

  if (expander.textContent === '+')
  {
    expander.textContent = '–';
    products.style.display = 'flex';
  }
  else
  {
    expander.textContent = '+';
    products.style.display = 'none';
    return; // don't load if collapsing
  }

  if (outfit.products.length === 0)
  {
    let index = 0;
    const limit = 5; // 👈 adjust concurrency here

    async function worker()
    {
      while (index < outfit.pids.length)
      {
        const currentIndex = index++;
        const pid = outfit.pids[currentIndex];

        const data = await getData(pid, 'product');

        let product;

        if (data != null)
        {
          product = {
            pid: pid,
            outfit_id: outfit.id,
            name: data.product_name,
            creator_cid: data.creator_cid,
            creator_name: data.creator_name,
            rating: data.rating,
            image: data.product_image,
            price: data.product_price,
            dummy: false,
            link: `https://www.imvu.com/shop/product.php?products_id=${pid}`
          };
        }
        else
        {
          const betterName = await getProductName(pid);
          product = {
            pid: pid,
            outfit_id: outfit.id,
            name: betterName,
            creator_cid: '0',
            creator_name: 'somebody',
            rating: '??',
            image: DEFAULT_IMAGE,
            price: -1,
            dummy: true,
            link: `https://www.imvu.com/shop/derivation_tree.php?products_id=${pid}`
          };
        }

        // ✅ progressive rendering
        outfit.products.push(product);
        generateProduct(product);
      }
    }

    const workers = [];
    for (let i = 0; i < limit; i++)
    {
      workers.push(worker());
    }

    await Promise.all(workers);
  }
}

function generateProduct(product)
{
  const container = document.getElementById(`products_${product.outfit_id}`);

  // build the UI
  const div_product = document.createElement('div');
  div_product.className = 'product';
  div_product.id = `product_${product.pid}`;

  const a1 = document.createElement("a");
  a1.href = product.link;
  a1.target = "_blank";
  const img = document.createElement("img");
  img.src = product.image;
  img.className = "product-img";
  a1.appendChild(img);
  div_product.appendChild(a1);

  const productInfo = document.createElement("div");
  productInfo.className = "product-info";
  const productName = document.createElement("div");
  productName.className = "product-name";
  const a2 = document.createElement("a");
  a2.href = product.link;
  a2.target = "_blnk";
  a2.textContent = product.name;
  productName.appendChild(a2);
  productInfo.appendChild(productName);

  const productCreator = document.createElement("div");
  productCreator.className = "creator";
  const textNode = document.createTextNode("by ");
  productCreator.appendChild(textNode);
  const a3 = document.createElement("a");
  a3.href = `https://www.imvu.com/shop/web_search.php?manufacturers_id=${product.creator_cid}`;
  a3.target = "_blank";
  a3.textContent = product.creator_name;
  productCreator.appendChild(a3);
  productInfo.appendChild(productCreator);

  const productDetails = document.createElement("div");
  productDetails.className = "product-details";
  if (product.rating === "AP")
  {
    const apSpan = document.createElement("span");
    apSpan.className = "product-ap";
    apSpan.textContent = "AP";
    productDetails.appendChild(apSpan);
  }
  const price = document.createElement("span");
  price.className = "product-price";
  price.textContent = product.price;
  productDetails.appendChild(price);
  productInfo.appendChild(productDetails);
  div_product.appendChild(productInfo);
  container.appendChild(div_product);
}

async function getData(id, item) {
  const url = `https://www.triggerless.com/imvu.php?${item}=${id}`;

    try {
      const response = await fetch(url);

        // Return null if not 200 OK
        if (!response.ok) {
          return null;
        }

      const json = await response.json();

      const firstEntry = Object.values(json.denormalized)[0];
      const data = firstEntry?.data ?? null;

      console.log(data);
      return data;
    } catch (err) {
        // Network errors, invalid JSON, etc.
      console.error("Fetch failed:", err);
      return null;
    }
}

async function getProductName(pid) {
  const url = `https://www.triggerless.com/product-name.php?pid=${pid}`;
  const DEFAULT_NAME = "Unknown/Hidden"
    try {
      const response = await fetch(url);

        // Return null if not 200 OK
        if (!response.ok) {
          return DEFAULT_NAME;
        }

      const json = await response.json();
      if (json.name) {
        return json.name;
      }
    } catch (err) {
        // Network errors, invalid JSON, etc.
      console.error("Fetch failed:", err);
      return DEFAULT_NAME;
    }
}

async function sendOutfitRequest(cid, blob)
{
  const url = "https://www.triggerless.com/api/outfits/request";

  const payload = {
    CustomerId: cid,
    Request: blob
  };

  try
  {
    const response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    });

    // Handle non-200 responses
    if (!response.ok)
    {
      const text = await response.text(); // backend might not return JSON on error
      throw new Error(`HTTP ${response.status}: ${text}`);
    }

    // Parse JSON response
    const data = await response.json();
    return data;
  }
  catch (err)
  {
    console.error("sendOutfitRequest failed:", err);
    return null;
  }
}
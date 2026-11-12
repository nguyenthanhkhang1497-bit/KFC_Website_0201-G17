/* =========================
   DU LIEU GIO HANG
========================= */

let cart = [];

/* =========================
   TIM KIEM SAN PHAM
========================= */

const searchForm = document.getElementById('search-form');
const searchInput = document.getElementById('search-input');
const productGrid = document.getElementById('product-grid');
const productCards = document.querySelectorAll('.product-card');
const noResult = document.getElementById('no-result');
const searchResultMsg = document.getElementById('search-result-msg');

// Xu ly submit form tim kiem
searchForm.addEventListener('submit', function (e) {
    e.preventDefault();
    filterProducts();
});

// Tim kiem theo thoi gian thuc (khi go phim)
searchInput.addEventListener('input', function () {
    filterProducts();
});

function filterProducts() {
    const keyword = searchInput.value.toLowerCase().trim();
    let visibleCount = 0;

    productCards.forEach(function (card) {
        const name = card.getAttribute('data-name').toLowerCase();
        const desc = card.querySelector('p') ? card.querySelector('p').textContent.toLowerCase() : '';

        if (name.includes(keyword) || desc.includes(keyword)) {
            card.classList.remove('hidden');
            visibleCount++;
        } else {
            card.classList.add('hidden');
        }
    });

    // Hien thi thong bao ket qua
    if (keyword === '') {
        noResult.style.display = 'none';
        searchResultMsg.textContent = '';
    } else if (visibleCount === 0) {
        noResult.style.display = 'block';
        searchResultMsg.textContent = 'Không tìm thấy kết quả cho "' + keyword + '"';
    } else {
        noResult.style.display = 'none';
        searchResultMsg.textContent = 'Tìm thấy ' + visibleCount + ' sản phẩm cho "' + keyword + '"';
    }

    // Reset danh muc ve "Tat ca"
    document.querySelectorAll('.category-menu a').forEach(function (a) {
        a.classList.remove('active');
    });
    document.querySelector('.category-menu a[data-category="all"]').classList.add('active');
}


/* =========================
   LOC THEO DANH MUC
========================= */

const categoryLinks = document.querySelectorAll('.category-menu a');

categoryLinks.forEach(function (link) {
    link.addEventListener('click', function (e) {
        e.preventDefault();

        // Xoa tu khoa tim kiem khi chon danh muc
        searchInput.value = '';
        searchResultMsg.textContent = '';
        noResult.style.display = 'none';

        // Doi trang thai active
        categoryLinks.forEach(function (a) {
            a.classList.remove('active');
        });
        this.classList.add('active');

        const category = this.getAttribute('data-category');

        productCards.forEach(function (card) {
            if (category === 'all' || card.getAttribute('data-category') === category) {
                card.classList.remove('hidden');
            } else {
                card.classList.add('hidden');
            }
        });

        // Cuon xuong phan san pham
        document.getElementById('san-pham').scrollIntoView({ behavior: 'smooth' });
    });
});


/* =========================
   GIO HANG - TOGGLE MO/DONG
========================= */

const cartSidebar = document.getElementById('cart-sidebar');
const cartOverlay = document.getElementById('cart-overlay');

function toggleCart() {
    cartSidebar.classList.toggle('open');
    cartOverlay.classList.toggle('open');
}


/* =========================
   GIO HANG - THEM SAN PHAM
========================= */

function addToCart(name, price, image) {
    // Kiem tra san pham da co trong gio chua
    const existingItem = cart.find(function (item) {
        return item.name === name;
    });

    if (existingItem) {
        existingItem.quantity++;
    } else {
        cart.push({
            name: name,
            price: price,
            image: image,
            quantity: 1
        });
    }

    updateCart();
    showToast('Đã thêm "' + name + '" vào giỏ hàng!');
}


/* =========================
   GIO HANG - CAP NHAT GIAO DIEN
========================= */

function updateCart() {
    const cartItemsContainer = document.getElementById('cart-items');
    const cartCount = document.getElementById('cart-count');
    const cartTotalPrice = document.getElementById('cart-total-price');
    const cartEmpty = document.getElementById('cart-empty');

    // Tinh tong so luong
    let totalQty = 0;
    let totalPrice = 0;

    cart.forEach(function (item) {
        totalQty += item.quantity;
        totalPrice += item.price * item.quantity;
    });

    // Cap nhat so luong tren icon
    cartCount.textContent = totalQty;

    // Cap nhat tong tien
    cartTotalPrice.textContent = formatPrice(totalPrice);

    // Xoa noi dung cu (giu lai cart-empty)
    cartItemsContainer.innerHTML = '';

    if (cart.length === 0) {
        const emptyMsg = document.createElement('p');
        emptyMsg.className = 'cart-empty';
        emptyMsg.id = 'cart-empty';
        emptyMsg.textContent = 'Giỏ hàng trống.';
        cartItemsContainer.appendChild(emptyMsg);
        return;
    }

    // Tao HTML cho tung san pham
    cart.forEach(function (item, index) {
        const cartItemDiv = document.createElement('div');
        cartItemDiv.className = 'cart-item';

        cartItemDiv.innerHTML =
            '<img src="' + item.image + '" alt="' + item.name + '">' +
            '<div class="cart-item-info">' +
                '<h4>' + item.name + '</h4>' +
                '<span class="cart-item-price">' + formatPrice(item.price * item.quantity) + '</span>' +
            '</div>' +
            '<div class="cart-item-qty">' +
                '<button onclick="changeQty(' + index + ', -1)">−</button>' +
                '<span>' + item.quantity + '</span>' +
                '<button onclick="changeQty(' + index + ', 1)">+</button>' +
            '</div>' +
            '<button class="cart-item-remove" onclick="removeItem(' + index + ')" title="Xóa">🗑️</button>';

        cartItemsContainer.appendChild(cartItemDiv);
    });

    // Luu vao localStorage
    saveCart();
}


/* =========================
   GIO HANG - TANG/GIAM SO LUONG
========================= */

function changeQty(index, delta) {
    cart[index].quantity += delta;

    if (cart[index].quantity <= 0) {
        cart.splice(index, 1);
    }

    updateCart();
}


/* =========================
   GIO HANG - XOA 1 MON
========================= */

function removeItem(index) {
    const name = cart[index].name;
    cart.splice(index, 1);
    updateCart();
    showToast('Đã xóa "' + name + '" khỏi giỏ hàng.');
}


/* =========================
   GIO HANG - XOA TAT CA
========================= */

function clearCart() {
    if (cart.length === 0) return;

    if (confirm('Bạn có chắc muốn xóa tất cả sản phẩm trong giỏ hàng?')) {
        cart = [];
        updateCart();
        showToast('Đã xóa tất cả sản phẩm trong giỏ hàng.');
    }
}


/* =========================
   GIO HANG - DAT HANG
========================= */

function checkout() {
    if (cart.length === 0) {
        showToast('Giỏ hàng trống! Hãy thêm sản phẩm trước.');
        return;
    }

    // Tinh tong tien
    let total = 0;
    let orderSummary = 'Đơn hàng của bạn:\n\n';

    cart.forEach(function (item) {
        total += item.price * item.quantity;
        orderSummary += '• ' + item.name + ' x' + item.quantity + ' = ' + formatPrice(item.price * item.quantity) + '\n';
    });

    orderSummary += '\nTổng cộng: ' + formatPrice(total);
    orderSummary += '\n\nCảm ơn bạn đã đặt hàng tại KFC Việt Nam! 🍗';

    alert(orderSummary);

    // Xoa gio hang sau khi dat
    cart = [];
    updateCart();
    toggleCart();
    showToast('Đặt hàng thành công! Cảm ơn bạn! 🎉');
}


/* =========================
   TOAST THONG BAO
========================= */

function showToast(message) {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.classList.add('show');

    setTimeout(function () {
        toast.classList.remove('show');
    }, 2500);
}


/* =========================
   DINH DANG GIA TIEN
========================= */

function formatPrice(price) {
    return price.toLocaleString('vi-VN') + 'đ';
}


/* =========================
   LUU & TAI GIO HANG TU LOCALSTORAGE
========================= */

function saveCart() {
    localStorage.setItem('kfc_cart', JSON.stringify(cart));
}

function loadCart() {
    const saved = localStorage.getItem('kfc_cart');
    if (saved) {
        cart = JSON.parse(saved);
        updateCart();
    }
}

// Tai gio hang khi mo trang
loadCart();

// Tải danh sách thực đơn từ Backend khi vào trang
async function fetchMenu() {
    try {
        const response = await fetch('/api/kfc/menu');
        const data = await response.json();
        console.log("Dữ liệu menu từ C#:", data);
        // Duyệt vòng lặp để render các thẻ card món ăn ra HTML tại đây
    } catch (err) {
        console.error("Lỗi tải menu:", err);
    }
}

// Gửi giỏ hàng lên Backend tính tổng tiền
async function updateCartSummary(cartItems) {
    const response = await fetch('/api/kfc/calculate-total', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(cartItems)
    });
    const summary = await response.json();
    console.log("Tổng tiền:", summary.GrandTotal);
}

// Đặt hàng
async function submitOrder(customerInfo, cartItems) {
    const payload = {
        customerName: customerInfo.name,
        phone: customerInfo.phone,
        address: customerInfo.address,
        items: cartItems
    };

    const response = await fetch('/api/kfc/checkout', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });
    const result = await response.json();
    alert(`Đặt hàng thành công! Mã đơn của bạn là: ${result.order.orderId}`);
}

// Tra cứu đơn hàng theo mã
async function trackOrder(orderId) {
    const response = await fetch(`/api/kfc/order-status/${orderId}`);
    if (response.ok) {
        const order = await response.json();
        alert(`Trạng thái đơn: ${order.status} - Tổng tiền: ${order.totalAmount} VNĐ`);
    } else {
        alert("Không tìm thấy đơn hàng!");
    }
}

// Gọi tải menu khi trang load xong
window.onload = fetchMenu;
/* =============================================================
   HÀM KẾT NỐI BACKEND: VOUCHER, ĐÁNH GIÁ, DOANH THU, PHÂN QUYỀN
============================================================= */
// Lấy danh sách thực đơn
async function fetchMenu() {
    try {
        const response = await fetch('/Home/GetMenu');
        const data = await response.json();
        console.log("Danh sách món từ MVC 5:", data);
    } catch (err) {
        console.error("Lỗi lấy menu:", err);
    }
}
// Kiểm tra mã giảm giá (ASP.NET MVC 5)
async function applyVoucher(code) {
    try {
        const res = await fetch(`/Home/CheckCoupon?code=${encodeURIComponent(code)}`);
        const data = await res.json();
        if (data.Success) {
            alert(`Áp dụng mã ${data.Coupon.Code} thành công! Giảm ${data.Coupon.DiscountPercent}% (Tối đa ${data.Coupon.MaxDiscountAmount.toLocaleString()}đ)`);
            return data.Coupon;
        } else {
            alert(data.Message);
        }
    } catch (err) {
        console.error("Lỗi áp dụng voucher:", err);
    }
}

// Gửi đánh giá món ăn
async function submitReview(menuItemId, name, rating, comment) {
    try {
        const res = await fetch('/api/Kfc/reviews', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ menuItemId, customerName: name, rating, comment })
        });
        const data = await res.json();
        alert(data.message);
    } catch (err) {
        console.error("Lỗi gửi đánh giá:", err);
    }
}

// Lấy thống kê doanh thu (Admin)
async function getRevenueStats() {
    try {
        const res = await fetch('/api/Kfc/admin/revenue');
        const data = await res.json();
        console.log("Thống kê doanh thu:", data);
        alert(`Tổng số đơn: ${data.totalOrders}\nTổng doanh thu: ${data.totalRevenue.toLocaleString()}đ\nTrung bình đơn: ${data.averageOrderValue.toLocaleString()}đ`);
    } catch (err) {
        console.error("Lỗi lấy doanh thu:", err);
    }
}

// Đăng nhập phân quyền nhân viên
async function loginStaff(username, password) {
    try {
        const res = await fetch('/api/Kfc/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });
        const data = await res.json();
        if (res.ok) {
            alert(`Xin chào ${data.user.fullName} (${data.user.role})!`);
            localStorage.setItem('user_role', data.user.role);
            return data.user;
        } else {
            alert(data.message);
        }
    } catch (err) {
        console.error("Lỗi đăng nhập:", err);
    }
}
document.addEventListener("DOMContentLoaded", function () {
    loadMenuItems();
});

// Hàm gọi API lấy danh sách món ăn từ database
function loadMenuItems() {
    fetch('/Home/GetMenuItems')
        .then(response => response.json())
        .then(res => {
            if (res.Success) {
                var container = document.getElementById('product-grid');
                if (!container) return;

                container.innerHTML = ''; // Làm sạch khung trước khi đổ dữ liệu

                res.Data.forEach(item => {
                    var card = `
                        <article class="product-card" data-category="${item.Category}" data-name="${item.Name}" data-price="${item.Price}">
                            <img src="${item.ImageUrl || '/Content/6.png.png'}" alt="${item.Name}" />
                            <div class="product-info">
                                <h3>${item.Name}</h3>
                                <p>${item.Description || ''}</p>
                                <strong>${Number(item.Price).toLocaleString('vi-VN')}đ</strong>
                                <button class="btn-add-cart" onclick="addToCart('${item.Name}', ${item.Price}, '${item.ImageUrl || '/Content/6.png.png'}')">
                                    Thêm vào giỏ
                                </button>
                            </div>
                        </article>
                    `;
                    container.innerHTML += card;
                });
            }
        })
        .catch(err => console.error("Lỗi tải món ăn từ database:", err));
    function checkoutOrder() {
        // Kiểm tra giỏ hàng rỗng (dựa theo biến cart trong script.js)
        if (typeof cart === 'undefined' || cart.length === 0) {
            alert("Giỏ hàng của bạn đang trống, vui lòng chọn món trước!");
            return;
        }

        var customerName = prompt("Vui lòng nhập Họ tên nhận hàng:");
        if (!customerName) return;

        var phone = prompt("Vui lòng nhập Số điện thoại:");
        if (!phone) return;

        var address = prompt("Vui lòng nhập Địa chỉ giao hàng:");
        if (!address) return;

        // Tính tổng tiền giỏ hàng
        var total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);

        var orderData = {
            CustomerName: customerName,
            Phone: phone,
            Address: address,
            TotalAmount: total,
            Items: cart.map(item => ({
                Name: item.name,
                Quantity: item.quantity,
                Price: item.price
            }))
        };

        fetch('/Home/CreateOrder', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(orderData)
        })
            .then(res => res.json())
            .then(res => {
                alert(res.Message);
                if (res.Success) {
                    // Xóa sạch giỏ hàng sau khi đặt thành công
                    cart = [];
                    if (typeof updateCartUI === 'function') updateCartUI();
                    if (typeof renderCart === 'function') renderCart();
                    location.reload();
                }
            })
            .catch(err => {
                console.error("Lỗi đặt hàng:", err);
                alert("Có lỗi xảy ra khi gửi đơn hàng!");
            });
    }
    function handleCheckout() {
        alert("Đã bấm được nút Đặt hàng!");
        // 1. Kiểm tra giỏ hàng
        if (typeof cart === 'undefined' || cart.length === 0) {
            alert("Giỏ hàng của bạn đang trống! Vui lòng chọn món trước.");
            return;
        }

        // 2. Nhập thông tin người nhận
        var customerName = prompt("Nhập họ tên người nhận:");
        if (!customerName) return;

        var phone = prompt("Nhập số điện thoại nhận hàng:");
        if (!phone) return;

        var address = prompt("Nhập địa chỉ giao hàng:");
        if (!address) return;

        // 3. Tính tổng tiền
        var total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);

        // 4. Đóng gói dữ liệu gửi về Backend
        var orderData = {
            CustomerName: customerName,
            Phone: phone,
            Address: address,
            TotalAmount: total,
            Items: cart.map(item => ({
                Name: item.name,
                Quantity: item.quantity,
                Price: item.price
            }))
        };

        // 5. Gửi request tạo đơn hàng
        fetch('/Home/CreateOrder', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(orderData)
        })
            .then(res => res.json())
            .then(res => {
                alert(res.Message);
                if (res.Success) {
                    cart = []; // Làm trống giỏ hàng
                    if (typeof updateCartUI === 'function') updateCartUI();
                    if (typeof renderCart === 'function') renderCart();
                    location.reload();
                }
            })
            .catch(err => {
                console.error("Lỗi đặt hàng:", err);
                alert("Không thể gửi đơn hàng!");
            });
    }
}
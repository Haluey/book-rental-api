let rentalHistoryData = [];
let showAllHistory = false;


// ==============================
// 공통 함수
// ==============================

async function fetchJson(url, options = {}) {
    const response = await fetch(url, options);

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message || '요청 처리 중 오류가 발생했습니다.');
    }

    return response.json();
}

function formatDate(dateString) {
    if (!dateString) {
        return '-';
    }

    const date = new Date(dateString);

    return date.toLocaleDateString('ko-KR');
}

function escapeHtml(value) {
    return String(value)
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#039;');
}


// ==============================
// 대여 현황
// ==============================

async function loadSummary() {
    try {
        const [books, availableBooks, activeRentals] =
            await Promise.all([
                fetchJson('/api/books'),
                fetchJson('/api/books/available'),
                fetchJson('/api/rentals/active')
            ]);

        document.getElementById('totalBooks').textContent = books.length;
        document.getElementById('availableBooks').textContent =
            availableBooks.length;
        document.getElementById('activeRentals').textContent =
            activeRentals.length;
    }
    catch (error) {
        console.error(
            '대여 현황 조회 중 오류가 발생했습니다.',
            error
        );
    }
}


// ==============================
// 현재 대여 중인 도서
// ==============================

async function loadActiveRentals() {
    const tableBody =
        document.getElementById('activeRentalTableBody');

    try {
        const rentals =
            await fetchJson('/api/rentals/active/details');

        if (rentals.length === 0) {
            tableBody.innerHTML = `
                <tr>
                    <td colspan="5" class="empty-message">
                        대여 중인 도서가 없습니다.
                    </td>
                </tr>
            `;

            return;
        }

        tableBody.innerHTML = rentals
            .map(rental => `
                <tr>
                    <td>${escapeHtml(rental.bookName)}</td>
                    <td>${escapeHtml(rental.memberName)}</td>
                    <td>${formatDate(rental.rentalDate)}</td>

                    <td>
                        <span class="status status-renting">
                            ${escapeHtml(rental.status)}
                        </span>
                    </td>

                    <td>
                        <button
                            type="button"
                            class="return-button"
                            data-rental-id="${rental.rentalIdx}"
                        >
                            반납
                        </button>
                    </td>
                </tr>
            `)
            .join('');
    }
    catch (error) {
        console.error(
            '현재 대여 목록 조회 중 오류가 발생했습니다.',
            error
        );
    }
}


// ==============================
// 회원 목록
// ==============================

async function loadMembers() {
    try {
        const members = await fetchJson('/api/members');

        const memberSelect =
            document.getElementById('memberSelect');

        memberSelect.innerHTML = `
            <option value="">회원을 선택하세요</option>
            ${members
                .map(member => `
                    <option value="${member.memberIdx}">
                        ${escapeHtml(member.memberName)}
                    </option>
                `)
                .join('')}
        `;
    }
    catch (error) {
        console.error(
            '회원 목록 조회 중 오류가 발생했습니다.',
            error
        );
    }
}


// ==============================
// 대여 가능한 도서
// ==============================

async function loadAvailableBooks() {
    try {
        const books =
            await fetchJson('/api/books/available');

        const bookSelect =
            document.getElementById('bookSelect');

        bookSelect.innerHTML = `
            <option value="">도서를 선택하세요</option>
            ${books
                .map(book => `
                    <option value="${book.bookIdx}">
                        ${escapeHtml(book.bookName)}
                    </option>
                `)
                .join('')}
        `;
    }
    catch (error) {
        console.error(
            '대여 가능 도서 조회 중 오류가 발생했습니다.',
            error
        );
    }
}


// ==============================
// 도서 대여
// ==============================

async function rentBook() {
    const memberSelect =
        document.getElementById('memberSelect');

    const bookSelect =
        document.getElementById('bookSelect');

    const memberIdx = memberSelect.value;
    const bookIdx = bookSelect.value;

    if (!memberIdx || !bookIdx) {
        alert('회원과 도서를 모두 선택해주세요.');
        return;
    }

    try {
        await fetchJson('/api/rentals', {
            method: 'POST',

            headers: {
                'Content-Type': 'application/json'
            },

            body: JSON.stringify({
                memberIdx: Number(memberIdx),
                bookIdx: Number(bookIdx)
            })
        });

        alert('도서가 대여되었습니다.');

        memberSelect.value = '';
        bookSelect.value = '';

        await refreshRentalData();
    }
    catch (error) {
        console.error(
            '도서 대여 중 오류가 발생했습니다.',
            error
        );

        alert(error.message);
    }
}


// ==============================
// 도서 반납
// ==============================

async function returnBook(rentalIdx) {
    const confirmed =
        confirm('이 도서를 반납 처리하시겠습니까?');

    if (!confirmed) {
        return;
    }

    try {
        const response =
            await fetch(
                `/api/rentals/${rentalIdx}/return`,
                {
                    method: 'PUT'
                }
            );

        if (!response.ok) {
            const message = await response.text();

            throw new Error(
                message || '도서 반납에 실패했습니다.'
            );
        }

        alert('도서가 반납되었습니다.');

        await refreshRentalData();
    }
    catch (error) {
        console.error(
            '도서 반납 중 오류가 발생했습니다.',
            error
        );

        alert(error.message);
    }
}


// ==============================
// 전체 대여 이력
// ==============================

async function loadRentalHistory() {
    try {
        const rentals =
            await fetchJson('/api/rentals/details');

        rentalHistoryData = rentals;

        document
            .getElementById('totalRentals')
            .textContent = rentals.length;

        filterRentalHistory();
    }
    catch (error) {
        console.error(
            '대여 이력 조회 중 오류가 발생했습니다.',
            error
        );
    }
}

function renderRentalHistory(rentals) {
    const tableBody =
        document.getElementById('rentalHistoryTableBody');

    if (rentals.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="5" class="empty-message">
                    검색 결과가 없습니다.
                </td>
            </tr>
        `;

        return;
    }

    const visibleRentals =
        showAllHistory
            ? rentals
            : rentals.slice(0, 5);

    tableBody.innerHTML = visibleRentals
        .map(rental => {
            const statusClass =
                rental.status === '대여중'
                    ? 'status-renting'
                    : 'status-returned';

            return `
                <tr>
                    <td>${escapeHtml(rental.bookName)}</td>
                    <td>${escapeHtml(rental.memberName)}</td>
                    <td>${formatDate(rental.rentalDate)}</td>
                    <td>${formatDate(rental.returnDate)}</td>

                    <td>
                        <span class="status ${statusClass}">
                            ${escapeHtml(rental.status)}
                        </span>
                    </td>
                </tr>
            `;
        })
        .join('');
}

function filterRentalHistory() {
    const keyword =
        document
            .getElementById('historySearch')
            .value
            .trim()
            .toLowerCase();

    const filtered =
        rentalHistoryData.filter(rental =>
            rental.bookName
                .toLowerCase()
                .includes(keyword) ||

            rental.memberName
                .toLowerCase()
                .includes(keyword)
        );

    renderRentalHistory(filtered);
}


// ==============================
// 대여 관련 화면 새로고침
// ==============================

async function refreshRentalData() {
    await Promise.all([
        loadSummary(),
        loadActiveRentals(),
        loadAvailableBooks(),
        loadRentalHistory()
    ]);
}


// ==============================
// 이벤트
// ==============================

document
    .getElementById('rentButton')
    .addEventListener('click', rentBook);

document
    .getElementById('activeRentalTableBody')
    .addEventListener('click', event => {
        const button =
            event.target.closest('.return-button');

        if (!button) {
            return;
        }

        const rentalIdx =
            Number(button.dataset.rentalId);

        returnBook(rentalIdx);
    });

document
    .getElementById('historySearch')
    .addEventListener(
        'input',
        filterRentalHistory
    );

document
    .getElementById('historyToggleButton')
    .addEventListener('click', function() {
        showAllHistory = !showAllHistory;

        this.textContent =
            showAllHistory
                ? '최근 5건 보기'
                : '전체 보기';

        filterRentalHistory();
    });


// ==============================
// 초기 로딩
// ==============================

async function initializePage() {
    await Promise.all([
        loadSummary(),
        loadActiveRentals(),
        loadMembers(),
        loadAvailableBooks(),
        loadRentalHistory()
    ]);
}

initializePage();
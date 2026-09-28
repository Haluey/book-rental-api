async function loadSummary() {
    try {
        const [booksResponse, availableResponse, activeResponse] =
            await Promise.all([
                fetch('/api/books'),
                fetch('/api/books/available'),
                fetch('/api/rentals/active')
            ]);

        const books = await booksResponse.json();
        const availableBooks = await availableResponse.json();
        const activeRentals = await activeResponse.json();

        document.getElementById('totalBooks').textContent = books.length;
        document.getElementById('availableBooks').textContent = availableBooks.length;
        document.getElementById('activeRentals').textContent = activeRentals.length;
    }
    catch (error) {
        console.error('대여 현황 조회 중 오류가 발생했습니다.', error);
    }
}

async function loadActiveRentals() {
    try {
        const response = await fetch('/api/rentals/active/details');
        const rentals = await response.json();

        const tableBody = document.getElementById('activeRentalTableBody');

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

        tableBody.innerHTML = rentals.map(rental => `
            <tr>
                <td>${rental.bookName}</td>
                <td>${rental.memberName}</td>
                <td>${formatDate(rental.rentalDate)}</td>
                <td>
                    <span class="status status-renting">${rental.status}</span>
                </td>
                <td>
                    <button
                        type="button"
                        class="return-button"
                        onclick="returnBook(${rental.rentalIdx})"
                    >
                        반납
                    </button>
                </td>
            </tr>
        `).join('');
    }
    catch (error) {
        console.error('현재 대여 목록 조회 중 오류가 발생했습니다.', error);
    }
}

function formatDate(dateString) {
    if (!dateString) {
        return '-';
    }

    const date = new Date(dateString);

    return date.toLocaleDateString('ko-KR');
}

async function loadMembers() {
    try {
        const response = await fetch('/api/members');
        const members = await response.json();

        const memberSelect = document.getElementById('memberSelect');

        memberSelect.innerHTML = `
            <option value="">회원을 선택하세요</option>
            ${members.map(member => `
                <option value="${member.memberIdx}">
                    ${member.memberName}
                </option>
            `).join('')}
        `;
    }
    catch (error) {
        console.error('회원 목록 조회 중 오류가 발생했습니다.', error);
    }
}

async function loadAvailableBooks() {
    try {
        const response = await fetch('/api/books/available');
        const books = await response.json();

        const bookSelect = document.getElementById('bookSelect');

        bookSelect.innerHTML = `
            <option value="">도서를 선택하세요</option>
            ${books.map(book => `
                <option value="${book.bookIdx}">
                    ${book.bookName}
                </option>
            `).join('')}
        `;
    }
    catch (error) {
        console.error('대여 가능 도서 조회 중 오류가 발생했습니다.', error);
    }
}

async function rentBook() {
    const memberSelect = document.getElementById('memberSelect');
    const bookSelect = document.getElementById('bookSelect');

    const memberIdx = memberSelect.value;
    const bookIdx = bookSelect.value;

    if (!memberIdx || !bookIdx) {
        alert('회원과 도서를 모두 선택해주세요.');
        return;
    }

    try {
        const response = await fetch('/api/rentals', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                memberIdx: Number(memberIdx),
                bookIdx: Number(bookIdx)
            })
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            alert(errorMessage);
            return;
        }

        alert('도서가 대여되었습니다.');

        memberSelect.value = '';
        bookSelect.value = '';

        await loadSummary();
        await loadActiveRentals();
        await loadAvailableBooks();
    }
    catch (error) {
        console.error('도서 대여 중 오류가 발생했습니다.', error);
        alert('도서 대여 중 오류가 발생했습니다.');
    }
}

async function returnBook(rentalIdx) {
    const confirmed = confirm('이 도서를 반납 처리하시겠습니까?');

    if (!confirmed) {
        return;
    }

    try {
        const response = await fetch(`/api/rentals/${rentalIdx}/return`, {
            method: 'PUT'
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            alert(errorMessage);
            return;
        }

        alert('도서가 반납되었습니다.');

        await loadSummary();
        await loadActiveRentals();
        await loadAvailableBooks();
    }
    catch (error) {
        console.error('도서 반납 중 오류가 발생했습니다.', error);
        alert('도서 반납 중 오류가 발생했습니다.');
    }
}

async function loadRentalHistory() {
    try {
        const response = await fetch('/api/rentals/details');
        const rentals = await response.json();

        const tableBody = document.getElementById('rentalHistoryTableBody');

        if (rentals.length === 0) {
            tableBody.innerHTML = `
                <tr>
                    <td colspan="5" class="empty-message">
                        대여 이력이 없습니다.
                    </td>
                </tr>
            `;
            return;
        }

        tableBody.innerHTML = rentals.map(rental => `
            <tr>
                <td>${rental.bookName}</td>
                <td>${rental.memberName}</td>
                <td>${formatDate(rental.rentalDate)}</td>
                <td>${formatDate(rental.returnDate)}</td>
                <td>
                    <span class="status ${
                                rental.status === '대여중'
                                    ? 'status-renting'
                                    : 'status-returned'
                                }">
                        ${rental.status}
                    </span>
                </td>
            </tr>
        `).join('');
    }
    catch (error) {
        console.error('대여 이력 조회 중 오류가 발생했습니다.', error);
    }
}

loadSummary();
loadActiveRentals();
loadMembers();
loadAvailableBooks();
loadRentalHistory();

document.getElementById('rentButton').addEventListener('click', rentBook);

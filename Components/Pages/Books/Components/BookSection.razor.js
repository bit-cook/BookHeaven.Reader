const observers = new Map();

function getLayout(section) {
    const list = section.querySelector('[data-book-list]');
    if (!list) return null;

    const cards = [...list.querySelectorAll('[data-book-card]')];
    if (!cards.length) return { list, cards: [] };

    const s = getComputedStyle(list);
    const gapX = parseFloat(s.columnGap || s.gap || '0');
    const gapY = parseFloat(s.rowGap || s.gap || '0');
    const { width: cardWidth, height: cardHeight } = cards[0].getBoundingClientRect();

    return { list, cards, gapX, gapY, cardWidth, cardHeight };
}

export function initialize(sectionId, dotNetReference) {
    const section = document.getElementById(sectionId);
    if (!section || !dotNetReference) return;

    refresh(sectionId, dotNetReference);

    const observer = new ResizeObserver(() => refresh(sectionId, dotNetReference));
    observer.observe(section);
    observers.set(sectionId, observer);
}

export function refresh(sectionId, dotNetReference) {
    const section = document.getElementById(sectionId);
    if (!section || !dotNetReference) return;

    const layout = getLayout(section);
    if (!layout) return;
    if (!layout.cards.length) {
        dotNetReference.invokeMethodAsync('UpdateVisibleBooks', 0);
        return;
    }

    const { list, gapX, gapY, cardWidth, cardHeight } = layout;
    const columns     = Math.max(1, Math.floor((list.clientWidth + gapX) / (cardWidth + gapX)));
    const visibleRows = Math.max(1, Math.floor((section.clientHeight + gapY) / (cardHeight + gapY)));

    dotNetReference.invokeMethodAsync('UpdateVisibleBooks', columns * visibleRows);
}

export function scrollToPage(sectionId, pageIndex, visibleBooksCount) {
    const section = document.getElementById(sectionId);
    if (!section || visibleBooksCount <= 0) return;

    const layout = getLayout(section);
    if (!layout || !layout.cards.length) return;

    const { list, gapY, cardHeight } = layout;
    list.style.transform = `translate3d(0, -${pageIndex * (cardHeight + gapY)}px, 0)`;
}

export function dispose(sectionId) {
    const observer = observers.get(sectionId);
    observer?.disconnect();
    observers.delete(sectionId);
}
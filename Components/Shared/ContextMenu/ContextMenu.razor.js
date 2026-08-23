let dotNetRef = null;
let isRegistered = false;

export function ctxPositionMenu(menu, x, y) {
    if (!menu) return;
    
    const margin = 8;
    const { width, height } = menu.getBoundingClientRect();
    const vw = window.innerWidth;
    const vh = window.innerHeight;

    let nx = x;
    let ny = y;

    if (x + width > vw - margin) {
        nx = Math.max(margin, vw - margin - width);
    }

    if (y + height > vh - margin) {
        ny = Math.max(margin, vh - margin - height);
    }

    menu.style.left = `${nx}px`;
    menu.style.top = `${ny}px`;
}

export function ctxRegisterOutsideClickHandler(dotNet) {
    if (isRegistered) return;

    dotNetRef = dotNet;
    document.addEventListener('mousedown', ctxHandleDocumentMouseDown);
    isRegistered = true;
}

export function ctxDisposeOutsideClickHandler() {
    if (!isRegistered) return;

    document.removeEventListener('mousedown', ctxHandleDocumentMouseDown);
    dotNetRef = null;
    isRegistered = false;
}

function ctxHandleDocumentMouseDown(event) {
    let menu = document.querySelector('.ctx-menu')
    if (menu.contains(event.target)) return;

    dotNetRef?.invokeMethodAsync('CloseMenuOnOutsideClick');
}

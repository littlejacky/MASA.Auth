// user
export function onUserFileterAnimationEnd() {
    var filterDocument = document.getElementById('userFilter');
    if (filterDocument) {
        filterDocument.onanimationend = () => {
            if (filterDocument.className.includes('showAnimation') || filterDocument.className.includes('closeAnimation')) {
                filterDocument.className = filterDocument.className.replace('showAnimation', '').replace('closeAnimation', '');
            }
        };
    }   
}
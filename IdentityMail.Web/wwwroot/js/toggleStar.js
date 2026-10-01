
async function toggleStar(id, element, isImportantPage) {
    const response = await fetch('/Message/MakeImportant/' + id);

    if (response.ok) {
        // Eğer bu "Önemli Mesajlar" sayfasındaysa:
        if (isImportantPage === true) {
            // 'closest' komutu, o yıldızın içinde bulunduğu en dış 'div' kutusunu bulur ve DOM'dan siler.
            element.closest('.group').remove();
        }
        // Eğer bu normal Gelen Kutusu sayfasındaysa (sadece renk değiştir):
        else {
            if (element.classList.contains('text-yellow-500')) {
                element.classList.remove('text-yellow-500');
                element.classList.add('text-outline-variant');
            } else {
                element.classList.remove('text-outline-variant');
                element.classList.add('text-yellow-500');
            }
        }
    }
}
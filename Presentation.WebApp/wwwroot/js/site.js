document.querySelectorAll('[data-accordion]').forEach(accordion => {
    const items = accordion.querySelectorAll('.accordion-item');

    items.forEach(item => {
        const trigger = item.querySelector('.accordion-trigger');

        if (trigger.getAttribute('aria-expanded') === 'true') {
            item.classList.add('is-open');
        }

        trigger.addEventListener('click', () => {
            const isExpanded = trigger.getAttribute('aria-expanded') === 'true';

            items.forEach(otherItem => {
                const otherTrigger = otherItem.querySelector('.accordion-trigger');
                otherTrigger.setAttribute('aria-expanded', 'false');
                otherItem.classList.remove('is-open');
            });

            if (!isExpanded) {
                trigger.setAttribute('aria-expanded', 'true');
                item.classList.add('is-open');
            }
        });
    });
});
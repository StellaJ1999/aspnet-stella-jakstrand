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

/*
    Realtidsvalidering för alla formulär som använder MVC TagHelpers + DataAnnotations.

*/

(() => {
    const cssEscape = (value) => {
        if (window.CSS?.escape) return window.CSS.escape(value);
        return String(value).replace(/["\\]/g, '\\$&');
    };

    const isBlank = (v) => !v || v.trim().length === 0;

    const isValidEmail = (value) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);

    const isValidPhone = (value) => /^[\d\s()+\-\.]{6,}$/.test(value);

    const getMessageSpan = (form, field) => {
        const name = field.getAttribute('name');
        if (!name) return null;
        return form.querySelector(`[data-valmsg-for="${cssEscape(name)}"]`);
    };

    const setFieldMessage = (form, field, message) => {
        const msg = getMessageSpan(form, field);
        if (!msg) return;

        msg.textContent = message ?? '';

        if (message) {
            msg.classList.add('field-validation-error');
            msg.classList.remove('field-validation-valid');
        } else {
            msg.classList.remove('field-validation-error');
            msg.classList.add('field-validation-valid');
        }
    };

    const getOtherFieldByExpression = (form, expression) => {
        // MVC brukar generera t.ex. "*.Password" i data-val-equalto-other.
        // Sista delen plockas ut och matchar på name som slutar med ".Password" eller exakt "Password".
        if (!expression) return null;

        const cleaned = expression.replace(/^[*]\./, '');
        const candidates = Array.from(form.querySelectorAll('input[name], textarea[name], select[name]'));
        return candidates.find(x => x.name === cleaned || x.name.endsWith(`.${cleaned}`)) ?? null;
    };

    const validateField = (form, field, touched, forceShowMessage = false) => {
        const name = field.getAttribute('name') ?? '';
        const value = field.value ?? '';
        const shouldShow = forceShowMessage || touched.has(name);

        if (!shouldShow) {
            setFieldMessage(form, field, '');
            return true;
        }

        // Required
        const requiredMsg = field.getAttribute('data-val-required');
        if (requiredMsg && isBlank(value)) {
            setFieldMessage(form, field, requiredMsg);
            return false;
        }

        // Email
        const emailMsg = field.getAttribute('data-val-email') || field.getAttribute('data-val-emailaddress');
        if (emailMsg && !isBlank(value) && !isValidEmail(value)) {
            setFieldMessage(form, field, emailMsg);
            return false;
        }

        // Phone
        const phoneMsg = field.getAttribute('data-val-phone');
        if (phoneMsg && !isBlank(value) && !isValidPhone(value)) {
            setFieldMessage(form, field, phoneMsg);
            return false;
        }

        // StringLength / Length
        const lengthMsg = field.getAttribute('data-val-length');
        const minStr = field.getAttribute('data-val-length-min');
        const maxStr = field.getAttribute('data-val-length-max');

        const min = minStr ? Number(minStr) : null;
        const max = maxStr ? Number(maxStr) : null;

        if (lengthMsg && !isBlank(value)) {
            if (min !== null && value.length < min) {
                setFieldMessage(form, field, lengthMsg);
                return false;
            }
            if (max !== null && value.length > max) {
                setFieldMessage(form, field, lengthMsg);
                return false;
            }
        }

        // Regex
        const regexMsg = field.getAttribute('data-val-regex');
        const regexPattern = field.getAttribute('data-val-regex-pattern');
        if (regexMsg && regexPattern && !isBlank(value)) {
            const re = new RegExp(regexPattern);
            if (!re.test(value)) {
                setFieldMessage(form, field, regexMsg);
                return false;
            }
        }

        // EqualTo (t.ex. ConfirmPassword)
        const equalToMsg = field.getAttribute('data-val-equalto');
        const equalToOther = field.getAttribute('data-val-equalto-other');
        if (equalToMsg && equalToOther) {
            const otherField = getOtherFieldByExpression(form, equalToOther);
            if (otherField && field.value !== otherField.value) {
                setFieldMessage(form, field, equalToMsg);
                return false;
            }
        }

        setFieldMessage(form, field, '');
        return true;
    };

    const initFormValidation = (form) => {
        // Initierar bara formulär som faktiskt har MVC validation-attribut.
        const fields = Array.from(form.querySelectorAll('input[data-val="true"][name], textarea[data-val="true"][name], select[data-val="true"][name]'));
        if (fields.length === 0) return;

        const touched = new Set();

        fields.forEach(field => {
            field.addEventListener('blur', () => {
                const name = field.getAttribute('name');
                if (name) touched.add(name);
                validateField(form, field, touched, true);
            });

            field.addEventListener('input', () => {
                // Realtidsvalidering medan man skriver, men först efter att fältet blivit "touched".
                validateField(form, field, touched, false);
            });
        });

        form.addEventListener('submit', (e) => {
            let allOk = true;

            fields.forEach(field => {
                const name = field.getAttribute('name');
                if (name) touched.add(name);
                if (!validateField(form, field, touched, true)) allOk = false;
            });

            if (!allOk) {
                e.preventDefault();
                e.stopPropagation();
            }
        });
    };

    document.querySelectorAll('form').forEach(initFormValidation);
})();


(() => {
    const TRANSITION_MS = 200;

    const buttons = document.querySelectorAll('#mobile-menu-button[data-target]');
    if (buttons.length === 0) return;

    const setOpen = (button, flyout, backdrop, isOpen) => {
        button.setAttribute('aria-expanded', isOpen ? 'true' : 'false');

        if (isOpen) {
            flyout.hidden = false;
            backdrop.hidden = false;

            requestAnimationFrame(() => flyout.classList.add('is-open'));
        } else {
            flyout.classList.remove('is-open');

            window.setTimeout(() => {
                flyout.hidden = true;
                backdrop.hidden = true;
            }, TRANSITION_MS);
        }
    };

    buttons.forEach(button => {
        const header = button.closest('.section__header') ?? document;
        const targetSelector = button.getAttribute('data-target');
        if (!targetSelector) return;

        const flyout = header.querySelector(targetSelector) ?? document.querySelector(targetSelector);
        const backdrop = header.querySelector('#mobile-menu-backdrop') ?? document.querySelector('#mobile-menu-backdrop');
        if (!flyout || !backdrop) return;

        const updateFlyoutPosition = () => {
            const rect = header.getBoundingClientRect();
            const top = Math.max(8, rect.bottom + 12); 
            flyout.style.top = `${top}px`;
        };

        let rafPending = false;
        const schedulePositionUpdate = () => {
            if (rafPending) return;
            rafPending = true;
            requestAnimationFrame(() => {
                rafPending = false;
                updateFlyoutPosition();
            });
        };

        const onViewportChange = () => {
            const isOpen = button.getAttribute('aria-expanded') === 'true';
            if (isOpen) schedulePositionUpdate();
        };

        button.setAttribute('aria-expanded', 'false');
        flyout.hidden = true;
        backdrop.hidden = true;
        flyout.classList.remove('is-open');

        button.addEventListener('click', () => {
            const isOpen = button.getAttribute('aria-expanded') === 'true';

            if (!isOpen) {
                updateFlyoutPosition();
                window.addEventListener('resize', onViewportChange);
                window.addEventListener('scroll', onViewportChange, { passive: true });
            } else {
                window.removeEventListener('resize', onViewportChange);
                window.removeEventListener('scroll', onViewportChange);
            }

            setOpen(button, flyout, backdrop, !isOpen);
        });

        backdrop.addEventListener('click', () => {
            window.removeEventListener('resize', onViewportChange);
            window.removeEventListener('scroll', onViewportChange);
            setOpen(button, flyout, backdrop, false);
        });

        flyout.addEventListener('click', (e) => {
            const link = e.target?.closest?.('a');
            if (!link) return;

            window.removeEventListener('resize', onViewportChange);
            window.removeEventListener('scroll', onViewportChange);
            setOpen(button, flyout, backdrop, false);
        });

        document.addEventListener('keydown', (e) => {
            if (e.key !== 'Escape') return;

            const isOpen = button.getAttribute('aria-expanded') === 'true';
            if (!isOpen) return;

            window.removeEventListener('resize', onViewportChange);
            window.removeEventListener('scroll', onViewportChange);
            setOpen(button, flyout, backdrop, false);
        });
    });
})();
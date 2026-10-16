
.site-header {
    position: sticky;
    top: 0;
    z-index: 1030;
    background: #fff;
    border-bottom: 3px solid #e31837;
    box-shadow: 0 3px 14px rgb(25 35 55 / 5%);
}

.site-navbar {
    min-height: 76px;
    padding: 12px 0;
    background: #fff;
}

.site-brand {
    display: inline-flex;
    align-items: center;
    gap: 9px;
    margin-right: 32px;
    padding: 4px 0;
    transition: opacity 180ms ease, transform 180ms ease;
}

.site-brand:hover {
    opacity: 0.85;
    transform: translateY(-1px);
}

.site-brand-mark {
    display: grid;
    width: 38px;
    height: 38px;
    place-items: center;
    border-radius: 11px;
    background: #e31837;
    color: #fff;
    font-size: 1.8rem;
    font-weight: 900;
    line-height: 1;
}

.site-brand-name {
    color: #e31837;
    font-size: 1.85rem;
    font-weight: 900;
    letter-spacing: -1.4px;
    line-height: 1;
}

.site-nav-link {
    position: relative;
    display: inline-flex;
    align-items: center;
    gap: 7px;
    margin-left: 8px;
    padding: 11px 15px !important;
    border-radius: 9px;
    color: #343946;
    font-size: 0.92rem;
    font-weight: 700;
    transition:
        color 180ms ease,
        background-color 180ms ease,
        transform 180ms ease;
}

.site-nav-link:hover,
.site-nav-link:focus-visible {
    transform: translateY(-1px);
    background: #fff0f2;
    color: #bd1230;
}

.site-nav-link:focus-visible,
.site-brand:focus-visible,
.site-navbar-toggler:focus-visible {
    outline: 3px solid rgb(227 24 55 / 30%);
    outline-offset: 3px;
}

.site-nav-link span {
    color: #e31837;
    font-size: 1.25rem;
}

.site-navbar-toggler {
    border: 1px solid #f1c4cc;
    border-radius: 9px;
    transition: background-color 180ms ease, border-color 180ms ease;
}

.site-navbar-toggler:hover {
    border-color: #e31837;
    background: #fff0f2;
}

@media (max-width: 767.98px) {
    .site-navbar {
        min-height: 66px;
    }

    .site-navbar .navbar-collapse {
        padding: 12px 0 4px;
    }

    .site-nav-link {
        display: flex;
        margin: 3px 0;
    }
}

@media (prefers-reduced-motion: reduce) {
    .site-brand,
    .site-nav-link,
    .site-navbar-toggler {
        transition: none;
    }

    .site-brand:hover,
    .site-nav-link:hover {
        transform: none;
    }
}

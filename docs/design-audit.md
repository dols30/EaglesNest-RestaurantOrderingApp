# Design audit

The original app had a muted red welcome screen, default purple buttons, gray title bars, green cart tiles, and inconsistent blue selection panels. It used Open Sans and the Eagles Nest Restaurant wordmark. There was no custom brand logo, web routing, analytics, or SEO metadata.

The ordering path is welcome → menu → category/combo → cart → checkout. Administration provides SQLite menu CRUD. These paths, prices, the 10% combo discount, and the 7% checkout tax are retained.

Problems: sparse homepage, unused name input, missing food image files, rigid category list and cart widths, checkout inside an unbounded scrolling collection, shared static cart IDs, invalid tips retaining stale totals, synchronous menu access, missing Apple scene configuration, and a debug interpreter setting that enabled AOT for every assembly.

Direction: bold, readable American diner kiosk using navy, orange and soft white; restrained interaction feedback; strong bundled food photography. The welcome page uses a responsive split layout and a compact explanation of the ordering steps. Selection screens use bounded collections, clear prices, and consistent selected states. Existing application naming and Shell route names stay stable.

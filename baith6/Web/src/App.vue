<template>
    <div id="app">
        <header class="site-header">
            <router-link to="/" class="brand" aria-label="LAB Insurance home">
                <img src="./assets/logo.png" alt="LAB Insurance" />
                <span><strong>LAB Insurance</strong><small>Sales portal</small></span>
            </router-link>
            <nav id="nav" aria-label="Main navigation">
                <router-link to="/" exact><i class="fa fa-home"></i><span>Home</span></router-link>
                <router-link to="/products"><i class="fa fa-shield"></i><span>Products</span></router-link>
                <router-link to="/policies"><i class="fa fa-file-text-o"></i><span>Policies</span></router-link>
                <router-link to="/dashboard"><i class="fa fa-bar-chart"></i><span>Dashboard</span></router-link>
                <router-link to="/chat"><i class="fa fa-comments-o"></i><span>Agent chat</span></router-link>
                <router-link to="/account"><i class="fa fa-user-circle-o"></i><span>Account</span></router-link>
            </nav>
        </header>

        <main id="main-container">
            <router-view />
        </main>

        <router-link to="/chatbot" class="btn-chat" id="livechat-compact-container">
            <i class="fa fa-commenting-o"></i><span>Live chat</span>
        </router-link>

        <footer class="site-footer">
            <img src="./assets/logo_horizontal.png" alt="Altkom Software & Consulting LAB" />
            <p>Insurance Sales Portal <span>·</span> Altkom Software &amp; Consulting LAB</p>
        </footer>
    </div>
</template>

<style>
    :root {
        --ink: #172b4d;
        --muted: #687b93;
        --navy: #102b4e;
        --teal: #08a88a;
        --canvas: #f4f7fb;
        --line: #e4eaf2;
    }
    * { box-sizing: border-box; }
    body { margin: 0; background: var(--canvas); color: var(--ink); font-family: 'Avenir', 'Segoe UI', Arial, sans-serif; -webkit-font-smoothing: antialiased; }
    #app { min-height: 100vh; display: flex; flex-direction: column; text-align: left; }
    .site-header { min-height: 78px; padding: 0 max(5vw, 24px); background: #fff; border-bottom: 1px solid var(--line); display: flex; align-items: center; justify-content: space-between; gap: 28px; box-shadow: 0 3px 14px rgba(23,43,77,.04); }
    .brand { display: inline-flex; align-items: center; gap: 12px; color: var(--navy); text-decoration: none !important; flex-shrink: 0; }
    .brand img { width: 42px; height: 42px; object-fit: contain; }
    .brand span { display: flex; flex-direction: column; line-height: 1.2; }
    .brand strong { font-size: 16px; letter-spacing: -.2px; }
    .brand small { margin-top: 4px; color: var(--muted); font-size: 11px; letter-spacing: 1.1px; text-transform: uppercase; }
    #nav { display: flex; align-items: center; gap: 5px; }
    #nav a { display: inline-flex; align-items: center; gap: 8px; padding: 11px 13px; border-radius: 9px; color: #62748b; font-size: 13px; font-weight: 600; text-decoration: none; transition: color .18s, background .18s; }
    #nav a .fa { font-size: 15px; }
    #nav a:hover { background: #f1f7f8; color: var(--teal); }
    #nav a.router-link-exact-active { background: #e8f7f4; color: #07866f; }
    #main-container { width: min(1180px, calc(100% - 48px)); margin: 38px auto 50px; flex: 1; }
    .site-footer { display: flex; align-items: center; justify-content: center; gap: 24px; padding: 24px; border-top: 1px solid var(--line); background: #fff; color: var(--muted); font-size: 12px; }
    .site-footer img { height: 36px; max-width: 180px; object-fit: contain; }
    .site-footer p { margin: 0; }
    .site-footer p span { padding: 0 5px; color: #b2bfcd; }
    .btn-chat { position: fixed; right: 22px; bottom: 22px; z-index: 1000; display: flex; align-items: center; gap: 9px; padding: 13px 18px; border-radius: 28px; background: var(--teal); box-shadow: 0 8px 22px rgba(8,168,138,.3); color: #fff !important; text-decoration: none !important; font-size: 13px; font-weight: 700; transition: transform .2s, background .2s; }
    .btn-chat:hover { background: #078d76; transform: translateY(-2px); }
    .btn-chat .fa { font-size: 17px; }
    h1, h2, h3, h4 { color: var(--navy); }
    .btn-primary { background-color: var(--teal); border-color: var(--teal); }
    .btn-primary:hover, .btn-primary:focus { background-color: #078d76; border-color: #078d76; }
    .card { border: 1px solid var(--line); border-radius: 14px; box-shadow: 0 6px 20px rgba(23,43,77,.06); overflow: hidden; }
    .form-control { border-color: #d9e2ed; border-radius: 8px; }
    .form-control:focus { border-color: #62cbb8; box-shadow: 0 0 0 .2rem rgba(8,168,138,.12); }
    @media (max-width: 900px) {
        .site-header { padding: 16px 24px; align-items: flex-start; flex-direction: column; gap: 12px; }
        #nav { width: 100%; flex-wrap: wrap; }
        #nav a { padding: 9px 11px; }
        #main-container { margin-top: 26px; }
    }
    @media (max-width: 560px) {
        #main-container { width: calc(100% - 28px); margin: 20px auto 38px; }
        .site-footer { flex-direction: column; gap: 10px; text-align: center; }
        #nav a { font-size: 12px; }
    }
</style>

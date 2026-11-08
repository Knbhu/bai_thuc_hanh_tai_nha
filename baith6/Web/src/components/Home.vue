<template>
    <div class="home-page">
        <section class="welcome-panel">
            <div class="welcome-copy">
                <div class="eyebrow"><span></span> INSURANCE SALES PLATFORM</div>
                <h1>{{ msg }}</h1>
                <p>A clear, connected workspace for insurance products, policies and sales performance.</p>
                <div class="welcome-actions">
                    <router-link to="/products" class="btn btn-primary"><i class="fa fa-th-large"></i> Explore products</router-link>
                    <router-link to="/dashboard" class="dashboard-link">View dashboard <i class="fa fa-arrow-right"></i></router-link>
                </div>
            </div>
            <div class="welcome-mark" aria-hidden="true">
                <div class="mark-halo"></div>
                <div class="shield"><i class="fa fa-shield"></i><i class="fa fa-check"></i></div>
                <div class="floating-chip chip-one"><i class="fa fa-file-text-o"></i> Policies</div>
                <div class="floating-chip chip-two"><i class="fa fa-line-chart"></i> Performance</div>
            </div>
        </section>

        <section class="platform-section">
            <div class="section-heading">
                <div><div class="eyebrow">BUILT FOR CONNECTED SERVICE</div><h2>One platform, every step in view</h2></div>
                <p>Powered by a microservice architecture<br/>for a responsive sales experience.</p>
            </div>
            <div class="architecture-frame">
                <img class="arch-image" alt="Insurance platform microservice architecture" src="https://raw.githubusercontent.com/asc-lab/dotnetcore-microservices-poc/master/readme-images/dotnetcore-microservices-architecture.png" />
            </div>
        </section>
    </div>
</template>

<script>
    export default {
        name: 'Home',
        props: { msg: String }
    }
</script>

<style scoped>
    .welcome-panel { position: relative; min-height: 320px; padding: 54px 64px; overflow: hidden; border-radius: 20px; background: linear-gradient(115deg, #102b4e 0%, #173d67 64%, #176a72 100%); color: #fff; box-shadow: 0 18px 38px rgba(16,43,78,.16); }
    .welcome-copy { position: relative; z-index: 2; max-width: 610px; }
    .eyebrow { display: flex; align-items: center; gap: 9px; color: #6bd7c3; font-size: 10px; font-weight: 800; letter-spacing: 1.5px; }
    .eyebrow span { width: 7px; height: 7px; border-radius: 50%; background: #6bd7c3; box-shadow: 0 0 0 4px rgba(107,215,195,.16); }
    h1 { margin: 20px 0 12px; color: #fff; font-size: clamp(30px, 4vw, 43px); line-height: 1.14; font-weight: 700; letter-spacing: -.8px; }
    .welcome-copy p { max-width: 490px; margin: 0; color: #c3d2e2; font-size: 15px; line-height: 1.7; }
    .welcome-actions { display: flex; align-items: center; gap: 24px; margin-top: 27px; }
    .welcome-actions .btn { padding: 11px 17px; border: 0; border-radius: 8px; font-size: 13px; font-weight: 700; }
    .welcome-actions .fa { margin-right: 7px; }
    .dashboard-link { color: #e1f4f1; font-size: 13px; font-weight: 600; text-decoration: none; }
    .dashboard-link .fa { margin-left: 5px; transition: transform .18s; }
    .dashboard-link:hover { color: #fff; text-decoration: none; }
    .dashboard-link:hover .fa { transform: translateX(3px); }
    .welcome-mark { position: absolute; right: 8%; top: 50%; width: 300px; height: 260px; transform: translateY(-50%); }
    .mark-halo { position: absolute; top: 2px; left: 27px; width: 245px; height: 245px; border: 1px solid rgba(180,230,225,.18); border-radius: 50%; box-shadow: 0 0 0 25px rgba(180,230,225,.035), 0 0 0 52px rgba(180,230,225,.025); }
    .shield { position: absolute; left: 94px; top: 55px; width: 112px; height: 126px; display: flex; align-items: center; justify-content: center; color: #72dec8; }
    .shield .fa-shield { font-size: 110px; filter: drop-shadow(0 12px 18px rgba(0,0,0,.2)); }
    .shield .fa-check { position: absolute; color: #fff; font-size: 34px; }
    .floating-chip { position: absolute; display: flex; align-items: center; gap: 8px; padding: 10px 13px; border: 1px solid rgba(255,255,255,.2); border-radius: 10px; background: rgba(255,255,255,.12); color: #f0f8fb; font-size: 11px; font-weight: 600; box-shadow: 0 10px 25px rgba(8,28,54,.15); backdrop-filter: blur(6px); }
    .floating-chip .fa { color: #78e0ca; }
    .chip-one { top: 44px; right: 0; }
    .chip-two { bottom: 38px; left: 0; }
    .platform-section { margin-top: 46px; }
    .section-heading { display: flex; align-items: flex-end; justify-content: space-between; gap: 24px; margin-bottom: 19px; }
    .section-heading h2 { margin: 9px 0 0; font-size: 23px; font-weight: 700; letter-spacing: -.35px; }
    .section-heading p { margin: 0 0 2px; color: #7a8ba0; font-size: 12px; line-height: 1.6; text-align: right; }
    .architecture-frame { padding: 22px; border: 1px solid #e4eaf2; border-radius: 14px; background: #fff; box-shadow: 0 6px 20px rgba(23,43,77,.045); }
    .arch-image { display: block; width: 100%; height: auto; }
    @media (max-width: 760px) {
        .welcome-panel { padding: 38px 30px; }
        .welcome-mark { right: -90px; opacity: .24; }
        .welcome-copy { max-width: 100%; }
        .section-heading { align-items: flex-start; flex-direction: column; gap: 8px; }
        .section-heading p { text-align: left; }
    }
    @media (max-width: 480px) {
        .welcome-panel { padding: 32px 23px; }
        .welcome-actions { align-items: flex-start; flex-direction: column; gap: 15px; }
        .section-heading h2 { font-size: 20px; }
        .architecture-frame { padding: 10px; overflow: auto; }
        .arch-image { min-width: 600px; }
    }
</style>

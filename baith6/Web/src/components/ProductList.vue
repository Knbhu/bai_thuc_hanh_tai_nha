<template>
    <div class="products-page">
        <div class="page-heading">
            <div><div class="eyebrow">COVERAGE THAT FITS</div><h1>Insurance products</h1><p>Explore available plans and choose the right protection.</p></div>
            <span class="product-count"><i class="fa fa-shield"></i> {{ products.length }} plans</span>
        </div>
        <div class="row product-grid">
        <div class="col-sm-6 col-lg-4 col-xl-3" v-for="product in products" :key="product.code">
            <ProductCard :product="product"></ProductCard>
        </div>
        </div>
    </div>
</template>

<script>
    import ProductCard from "./ProductCard";
    import {HTTP} from "./http/ApiClient";

    export default {
        name: 'ProductList',
        components: {ProductCard},
        data() {
            return {
                products: []
            };
        },
        created: function () {
            HTTP.get('products').then(response => {
                this.products = response.data;
            });
        }
    }
</script>

<style scoped lang="scss">
    .page-heading { display: flex; align-items: flex-end; justify-content: space-between; margin-bottom: 26px; }
    .eyebrow { color: #08a88a; font-size: 10px; font-weight: 800; letter-spacing: 1.4px; }
    h1 { margin: 7px 0 5px; font-size: 29px; font-weight: 700; }
    .page-heading p { margin: 0; color: #718198; font-size: 14px; }
    .product-count { padding: 9px 13px; border: 1px solid #deebe9; border-radius: 20px; background: #eff9f7; color: #07866f; font-size: 12px; font-weight: 700; }
    .product-count .fa { margin-right: 5px; }
    .product-grid > div { display: flex; margin-bottom: 24px; }
    @media (max-width: 560px) { .page-heading { align-items: flex-start; flex-direction: column; gap: 14px; } }
</style>

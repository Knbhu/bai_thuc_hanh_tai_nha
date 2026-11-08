<template>
    <b-card class="product-card" :title=product.name
            :img-src=product.image
            :img-alt=product.name
            img-top
            tag="article">
        <p class="card-text">
            {{product.description}}
            <CoverList :covers="product.covers"></CoverList>
        </p>
        <router-link :to="{name: 'product', params: { productCode: product.code }}">
            <b-button type="submit" variant="primary">View plan <i class="fa fa-arrow-right"></i></b-button>
        </router-link>
    </b-card>
</template>

<script>
    import CoverList from "./CoverList";

    export default {
        name: 'ProductCard',
        props: ['product'],
        components: {CoverList}
    }
</script>

<style scoped lang="scss">
    .product-card { width: 100%; display: flex; flex-direction: column; transition: transform .2s ease, box-shadow .2s ease; }
    .product-card:hover { transform: translateY(-4px); box-shadow: 0 14px 30px rgba(23,43,77,.12); }
    .card-img-top {
        height: 178px;
        object-fit: cover;
        background: #eaf1f5;
    }
    .card-body { display: flex; flex: 1; flex-direction: column; align-items: flex-start; padding: 20px; }
    .card-title { font-size: 17px; font-weight: 700; }
    .card-text { flex: 1; color: #6b7c91; font-size: 13px; line-height: 1.65; }
    .btn { margin-top: 14px; border-radius: 7px; font-size: 12px; font-weight: 700; }
    .btn .fa { margin-left: 7px; }
</style>

<template>
    <div>
        <div class="page-heading">
            <div><div class="eyebrow">POLICY MANAGEMENT</div><h1>Policies</h1><p>Find and review issued insurance policies.</p></div>
        </div>
        <div class="filter-container">
            <div class="filter-title"><i class="fa fa-search"></i><span>Search policies</span></div>
            <b-form inline>
                <b-input v-model="filterFields.number" placeholder="Policy number" class="mr-sm-2 mb-sm-0 search-input"/>
                <b-input v-model="filterFields.policyHolder" placeholder="Policy Holder" class="mr-sm-2 mb-sm-0 search-input"/>
                <b-button v-on:click="search()" variant="primary" class="search-button">Search</b-button>
            </b-form>
        </div>

        <b-table class="policy-table" bordered striped hover responsive
                 :items="policies"
                 :fields="fields"
                 @row-clicked="showDetails">
        </b-table>
    </div>
</template>

<script>
    import {HTTP} from "./http/ApiClient";
    const AND = "%20AND%20";

    export default {
        name: "PolicyList",
        data() {
            return {
                fields: [
                    {key: 'policyNumber'},
                    {key: 'policyStartDate'},
                    {key: 'policyEndDate'},
                    {key: 'policyHolder'}
                ],
                policies: [],
                filterFields: {
                    policyHolder: '',
                    number: ''
                }
            }
        },
        created: function () {
            this.runSearch();
        },
        methods: {
            showDetails(record) {
                this.$router.push({name: 'policyDetails', params: {policyNumber: record.policyNumber}});
            },
            search() {
                let queryString = '';
                queryString = this.addCriteria(queryString, this.filterFields.policyHolder);
                queryString = this.addCriteria(queryString, this.filterFields.number);

                this.runSearch(this.formatQueryString(queryString));
            },
            runSearch(queryString = '') {
                HTTP.get('PolicySearch' + queryString).then(response => {
                    this.policies = response.data.policies;
                });
            },
            addCriteria(queryString, criteria) {
                if(criteria !== '')
                    queryString += criteria + AND;

                return queryString;
            },
            formatQueryString(queryString) {
                if(queryString.endsWith(AND))
                    queryString = queryString.substring(0, queryString.length - AND.length);

                console.log(queryString);
                return queryString !== '' ? '?q=' + queryString : '';
            }
        }
    }
</script>

<style scoped>
    .page-heading { margin-bottom: 24px; }
    .eyebrow { color: #08a88a; font-size: 10px; font-weight: 800; letter-spacing: 1.4px; }
    h1 { margin: 7px 0 5px; font-size: 29px; font-weight: 700; }
    .page-heading p { margin: 0; color: #718198; font-size: 14px; }
    .filter-container { margin-bottom: 20px; padding: 20px; border: 1px solid #e4eaf2; border-radius: 12px; background: #fff; box-shadow: 0 5px 16px rgba(23,43,77,.045); }
    .filter-title { display: flex; align-items: center; gap: 9px; margin-bottom: 14px; color: #233b5c; font-size: 14px; font-weight: 700; }
    .filter-title .fa { color: #08a88a; }

    .search-input {
        width: min(300px, 36%) !important;
        margin-right: 10px;
        margin-bottom: 8px;
    }

    .search-button {
        min-width: 100px;
        margin-bottom: 8px;
    }
    .policy-table { overflow: hidden; border-radius: 10px; background: #fff; box-shadow: 0 5px 16px rgba(23,43,77,.045); }
    @media (max-width: 600px) { .search-input { width: 100% !important; margin: 0 0 9px; } .search-button { width: 100%; } }
</style>

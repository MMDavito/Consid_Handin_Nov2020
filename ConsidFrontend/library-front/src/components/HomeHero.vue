<template>
    <v-container fluid fill-height class="home-hero" style="max-height: 100vh;">
        <v-layout justify-center align-center column pa-5>
            <div class="display-4 font-weight-black white--text text-xs-center">
                Great reading experiences
            </div>
            <div
                class="display-4 font-weight-black white--text text-xs-center mb-3"
            >
                For all possible needs
            </div>
            <div class="display-1 font-weight-bold white--text text-xs-center">
                Place-holder. Lorem ipsimus or sumething.
            </div>
            <!--
            <v-btn fab class="mt-5 brown darken-4">
                <v-icon large color="white">expand_more</v-icon>
            </v-btn>
            -->
        </v-layout>
    </v-container>
</template>

<script>
export default {
    name: 'HomeHero'
};
</script>

<style scoped>
.home-hero {
    background: url('https://images.unsplash.com/photo-1536965764833-5971e0abed7c?ixlib=rb-1.2.1&w=1000&q=80'); /*nobody pays me to do photography*/
    background-size: cover;
    width: 100%;
    height: 100%;
}
</style>

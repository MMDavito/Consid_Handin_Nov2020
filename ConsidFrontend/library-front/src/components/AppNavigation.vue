<template>
    <span>
        <!--
        The navigation-drawer is abit buggy, requires to click to close it even though window size shows the regular 
        buttons.
        !-->
        <v-navigation-drawer
            app
            v-model="drawer"
            class="brown lighten-2"
            dark
            disable-resize-watcher
        >
            <v-list>
                <template v-for="(item, index) in items">
                    <v-list-item :key="index">
                        <v-btn text @click="$router.push(item.onclick)">{{
                            item.title
                        }}</v-btn>
                        <!--
                        <v-list-item-content>
                            {{ item.title }}
                        </v-list-item-content>
                        !-->
                    </v-list-item>
                    <v-divider :key="`divider-${index}`"></v-divider>
                </template>
            </v-list>
        </v-navigation-drawer>
        <v-app-bar app color="brown darken-4" dark>
            <v-btn class="hidden-md-and-up" @click="drawer = !drawer"
                >Drop-Down</v-btn
            >
            <v-spacer class="hidden-md-and-up"></v-spacer>
            <v-toolbar-title>{{ appTitle }}</v-toolbar-title>
            <v-btn text class="hidden-sm-and-down" @click="$router.push('/')"
                >Home</v-btn
            >
            <v-btn
                text
                class="hidden-sm-and-down"
                @click="$router.push('library_items')"
                >Library Items</v-btn
            >
            <v-btn
                text
                class="hidden-sm-and-down"
                @click="$router.push('/categories')"
                >Categories</v-btn
            >
            <v-btn
                text
                class="hidden-sm-and-down"
                @click="$router.push('create_library_items')"
                >Create Library Items</v-btn
            >
        </v-app-bar>
    </span>
</template>

<script>
export default {
    name: 'AppNavigation',
    data() {
        return {
            appTitle: 'Library',
            drawer: false,
            items: [
                { title: 'Home', onclick: '/' },
                { title: 'Library Item', onclick: 'library_items' },
                {
                    title: 'Categories',
                    onclick: 'categories'
                },
                {
                    title: 'Create Library Item',
                    onclick: 'create_library_items'
                }
            ]
        };
    }
};
</script>

<style scoped></style>

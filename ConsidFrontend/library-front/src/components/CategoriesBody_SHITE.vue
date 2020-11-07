<template>
    <Page class="page">
        <ActionBar class="action-bar" title="Pokemon"></ActionBar>
        <GridLayout>
            <!--
            <ListView for="p in pokemon" class="list-group">
                <v-template>
                    <StackLayout class="list-group-item">
                        <Label :text="p.name" />
                    </StackLayout>
                </v-template>
            </ListView>
            !-->
            <ul id="example-1">
                <li v-for="p in pokemon" :key="p.name">
                    {{ p.name }}
                </li>
            </ul>
        </GridLayout>
    </Page>
</template>

<script>
export default {
    data() {
        return {
            pokemon: []
        };
    },
    mounted() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };
        fetch('https://pokeapi.co/api/v2/pokemon/?limit=151', requestOptions)
            .then(response => response.text())
            .then(result => {
                var obj = JSON.parse(result);
                /*
                console.log('pokemons');
                console.log(obj);
                console.log(obj.results);

                console.log(obj.results[0]);
                */
                this.pokemon = obj.results;
            })
            .catch(error => console.log('error', error));
    }
};
</script>

<style scoped></style>

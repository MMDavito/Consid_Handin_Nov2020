<template>
    <v-container>
        <form>
            <v-layout column>
                <v-flex class="display-2 text-xs-center my-5"
                    >Big Title Goes Here</v-flex
                >
                <v-flex>
                    <div>
                        <label>{{ library_item.title }}</label
                        ><br />
                        <input
                            class="input"
                            id="title"
                            type="text"
                            :value="library_item.title"
                            placeholder="Title"
                        />
                        <select v-model="selected_category">
                            <option
                                v-for="c in categories"
                                v-bind:key="c.category"
                                v-bind:value="c.id"
                            >
                                {{ c.category }}
                            </option>
                        </select>
                        <!--
                        <span>Selected: {{ selected_category }}</span>
                        !-->
                        <input
                            v-if="
                                library_item.type == 'Book' ||
                                    library_item.type == 'Reference Book'
                            "
                            type="text"
                            id="author"
                            :value="library_item.author"
                            placeholder="Author"
                        />
                        <input
                            v-if="
                                library_item.type == 'Book' ||
                                    library_item.type == 'Reference Book'
                            "
                            type="number"
                            id="pages"
                            :value="library_item.pages"
                            placeholder="Pages"
                        />
                        <input
                            v-if="
                                library_item.type == 'DVD' ||
                                    library_item.type == 'Audio Book'
                            "
                            type="number"
                            id="runTimeMinutes"
                            :value="library_item.runTimeMinutes"
                            placeholder="Length Minutes"
                        />
                        <input
                            class="input"
                            id="borrower"
                            type="text"
                            :value="library_item.borrower"
                            placeholder="BORROWED BY"
                        />
                        <input
                            class="input"
                            id="'borrow_date"
                            type="date"
                            :value="library_item.borrow_date"
                        />
                        <input />
                        <br />
                        <v-btn
                            color="primary"
                            class="update"
                            :name="library_item.id"
                            @click="update(library_item.id)"
                            >Edit</v-btn
                        >
                        <v-btn
                            color="error"
                            class="delete"
                            :name="library_item.id"
                            @click="remove(library_item.id)"
                            >Delete</v-btn
                        >
                        <br />
                    </div>
                </v-flex>
            </v-layout>
        </form>
    </v-container>
</template>

<script>
export default {
    name: 'LibraryItemBody',
    data() {
        return {
            library_item: {},
            selected_category: null,
            categories: []
        };
    },
    mounted() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };
        console.log(
            'Will now querry server for library_item: ' + this.$route.params.id
        ); //Could probably use session/routerscope, but this works.
        var url =
            'https://127.0.0.1:5001/library_item/' + this.$route.params.id;
        fetch(url, requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.library_item = result[0];
                this.selected_category = this.library_item.categoryId;
            })
            .catch(error => console.log('error', error));
        url = 'https://127.0.0.1:5001/category/all';
        fetch(url, requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.categories = result;
            })
            .catch(error => console.log('error', error));
    },
    methods: {
        update: function(clicked_id) {
            console.log('Clicked: ' + clicked_id);
            var input_field = document.getElementById(clicked_id);
            console.log(input_field);
            var newData = input_field.value;
            console.log(newData);
            console.log(this.categories);
            var tempCat = this.categories[clicked_id];
            console.log('New data: ' + newData);
            console.log('Old data: ' + tempCat.category);
            //"check input" but realy just change color of input field and output info to log.
            //Possible that alert would be better, or something else, but alert is terrible when developing
            //Next time i will possibly use alert "if not HelperGlobalVariable.ISDEVELOPING is false"
            if (
                newData == null ||
                newData == undefined ||
                !(newData.length > 0) ||
                newData.length > 200 ||
                newData == tempCat.category
            ) {
                console.log(
                    'New category cannot have an empty name or a name longer than 200 bytes.\nNore can it be the same as already existing'
                );
                document.getElementById(clicked_id).style.backgroundColor =
                    'red';
                return;
            } else if (tempCat.id == null) {
                //Following is caused by lazzyness when creating .net response api:
                document.getElementById(clicked_id).style.backgroundColor =
                    'red';
                alert(
                    'You must reload page before updateing newely created category.'
                );
                return;
            } else {
                document.getElementById(
                    clicked_id
                ).style.backgroundColor = null;
            }
            var myHeaders = new Headers();
            myHeaders.append('Content-Type', 'application/json');

            //var category = null;
            console.log(tempCat);
            tempCat.category = newData;
            console.log(tempCat);
            var raw = JSON.stringify(tempCat);
            console.log('New category is: ' + raw);
            raw = JSON.stringify(raw);
            console.log('New category is: ' + raw);

            var requestOptions = {
                method: 'PUT',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };
            var url = 'https://127.0.0.1:5001/category/' + tempCat.id;
            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    console.log('Status var: ' + result.statusCode);
                    if (result.statusCode != 200) {
                        console.log('Will change colur');
                        console.log(document.getElementById(clicked_id));
                        document.getElementById(
                            clicked_id
                        ).style.backgroundColor = 'red';
                        console.log(document.getElementById(clicked_id));
                    } else {
                        this.$set(this.categories, clicked_id, {
                            id: tempCat.id,
                            category: tempCat.category
                        });
                        document.getElementById(
                            clicked_id
                        ).style.backgroundColor = null;
                    }
                })
                .catch(error => console.log('HELVETES ERROR SKIT', error));
        }
    }
};
</script>

<style scoped></style>

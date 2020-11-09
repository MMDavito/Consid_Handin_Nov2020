<template>
    <v-container>
        <form>
            <v-layout column>
                <v-flex class="display-2 text-xs-center my-5"
                    >Please fill all fields before pressing "Create"</v-flex
                >
                <v-flex>
                    <div>
                        <label for="title">Title: </label>
                        <input
                            class="input"
                            id="title"
                            type="text"
                            placeholder="Title"
                        />
                        <label for="category_select">Category: </label>
                        <select
                            v-model="selected_category"
                            id="category_select"
                        >
                            <option
                                v-for="c in categories"
                                v-bind:key="c.category"
                                v-bind:value="c.id"
                            >
                                {{ c.category }}
                            </option>
                        </select>
                        <label
                            for="author"
                            v-if="
                                selected_type == 'Book' ||
                                    selected_type == 'Reference Book'
                            "
                            >Author:
                        </label>
                        <input
                            v-if="
                                selected_type == 'Book' ||
                                    selected_type == 'Reference Book'
                            "
                            type="text"
                            id="author"
                            placeholder="Author"
                        />
                        <label
                            for="pages"
                            v-if="
                                selected_type == 'Book' ||
                                    selected_type == 'Reference Book'
                            "
                            >Pages:
                        </label>
                        <input
                            v-if="
                                selected_type == 'Book' ||
                                    selected_type == 'Reference Book'
                            "
                            type="number"
                            id="pages"
                            placeholder="Pages"
                            min="1"
                        />
                        <label
                            for="runTimeMinutes"
                            v-if="
                                selected_type == 'DVD' ||
                                    selected_type == 'Audio Book'
                            "
                            >Length minutes:
                        </label>
                        <input
                            v-if="
                                selected_type == 'DVD' ||
                                    selected_type == 'Audio Book'
                            "
                            type="number"
                            id="runTimeMinutes"
                            placeholder="Length Minutes"
                            min="1"
                        />
                        <label for="selectType">Type: </label>
                        <select v-model="selected_type" id="selectType">
                            <option
                                v-for="type in types"
                                v-bind:key="type"
                                v-bind:value="type"
                            >
                                {{ type }}
                            </option>
                        </select>
                        <br />
                        <v-btn color="primary" class="create" @click="create()"
                            >Create</v-btn
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
    name: 'CreateLibraryItem_Body',
    data() {
        return {
            selected_category: null,
            categories: [],
            types: ['Book', 'DVD', 'Audio Book', 'Reference Book'],
            selected_type: 'Book'
        };
    },
    mounted() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };
        var url = 'https://127.0.0.1:5001/category/all';
        fetch(url, requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.categories = result;
            })
            .catch(error => console.log('error', error));
    },
    methods: {
        create: function() {
            var temp = document.getElementById('title');
            var title = temp == null ? null : temp.value;
            var author = null;
            var pages = null;
            var runTimeMinutes = null;
            if (
                this.selected_type == 'Book' ||
                this.selected_type == 'Reference Book'
            ) {
                temp = document.getElementById('author');
                if (temp == null || temp.value.length == 0) {
                    document.getElementById('author').style.backgroundColor =
                        'red';
                    return;
                } else
                    document.getElementById(
                        'author'
                    ).style.backgroundColor = null;
                author = temp.value;

                temp = document.getElementById('pages');
                if (temp == null || temp.value <= 0) {
                    document.getElementById('pages').style.backgroundColor =
                        'red';
                    return;
                } else
                    document.getElementById(
                        'pages'
                    ).style.backgroundColor = null;
                pages = temp.value;
            } else if (
                this.selected_type == 'DVD' ||
                this.selected_type == 'Audio Book'
            ) {
                temp = document.getElementById('runTimeMinutes');
                if (runTimeMinutes <= 0) {
                    document.getElementById(
                        'runTimeMinutes'
                    ).style.backgroundColor = 'red';
                    return;
                } else
                    document.getElementById(
                        'runTimeMinutes'
                    ).style.backgroundColor = null;
            } else {
                alert("Can't use invalid types");
                return;
            }

            //"check input" but realy just change color of input field and output info to log.
            //Possible that alert would be better, or something else, but alert is terrible when developing
            //Next time i will possibly use alert "if not HelperGlobalVariable.ISDEVELOPING is false"

            var myHeaders = new Headers();
            myHeaders.append('Content-Type', 'application/json');
            var obj = {};
            obj.title = title;
            obj.author = author;
            obj.pages = pages;
            obj.runTimeMinutes = runTimeMinutes;
            obj.type = this.selected_type;
            obj.categoryId = this.selected_category;
            obj.borrower = null;
            obj.borrowDate = null;
            obj.isBorrowable = obj.type == 'Reference Book' ? false : true;

            //Borrow won't change, nore will type, since "onChange"
            //var category = null;
            var raw = JSON.stringify(obj);
            console.log('Item to create is: ' + raw);
            raw = JSON.stringify(raw);
            console.log('Restringed item is: ' + raw);

            var requestOptions = {
                method: 'POST',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };
            var url = 'https://127.0.0.1:5001/library_item/';
            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    console.log('Status var: ' + result.statusCode);
                    if (result.statusCode != 201) {
                        console.log('Will change colur');
                        alert(
                            'Failed updateing: Possibly will print error, but probably not, possibly to long input.\nOr lacking correct category'
                        );
                    } else {
                        alert('WILL reroute now!');
                        this.$router.push('library_items');
                    }
                })
                .catch(error => console.log('HELVETES ERROR SKIT', error));
        } /*,
        changed: function() {
            if (this.library_item.type == 'Reference Book') {
                this.library_item.type = this.selected_type;
                this.$set(this.library_item, 'type', this.selected_type);

                //The page count and author may be existing when going from reference book to dvd. But that's just a fun bug.
                //#easterEgg
                return;
            } //else
            else if (this.selected_type == 'Reference Book') {
                if (this.library_item.borrower != null) {
                    alert(
                        'Cannot change type to reference book when the book is already borrowed, you need to check it back in first.'
                    );
                    this.selected_type = this.library_item.type;
                    return;
                } else this.$set(this.library_item, 'type', this.selected_type);
            } else {
                this.$set(this.library_item, 'type', this.selected_type);
            }
        }*/
    }
};
</script>

<style scoped></style>

<template>
  <div id="app">
    <b-container>
      <b-row>
        <b-col class="mb-4" md="2">
          <button @click="logout()"><b-icon class="h2 mb-0" icon="box-arrow-in-left" variant="danger"></b-icon> Выйти</button>
        </b-col>
      </b-row>
      <b-row>
        <b-col offset-md="1" md="3">
          <b-button @click="deleteRecordsFromdb()" block variant="danger">Удалить все записи</b-button>
        </b-col>
        <b-col md="4">
          <b-button block @click="startButton()" :variant="buttonVariant"> {{buttonText}}</b-button>
        </b-col>
        <b-col md="3">
          <b-button block :variant="buttonVariant"> Счётчик: {{counter}}</b-button>
        </b-col>
      </b-row>
      <b-row>
        <b-col style="margin-top: 30px;" md="2">
          <b-form-input class="mb-4" style="width: 350px;" size="lg" 
            type="text" 
            placeholder="Фильтр по дате/времени событию"  
            v-model="filter" autofocus>
          </b-form-input>
          <div class="table-container">
          <table class="table table-dark table-hover">
            <thead>
              <th>Дата/время</th>
              <th>Событие</th>
              <th>Координаты x : y</th>
            </thead>
            <tbody>
              <tr v-for="(event, k) in filteredRecords.reverse().slice()" :key="k">
                <td>
                  {{ new Date(event.createdOn).toLocaleDateString("ru-RU") }}
                  {{ new Date(event.createdOn).toLocaleTimeString([], localTimeOptions) }}
                </td>
                <td>{{event.eventType}}</td>
                <td>{{event.coords}}</td>
              </tr>
            </tbody>
          </table>
          </div>
        </b-col>
        <b-col style="margin-top: 30px;" offset-md="2" md="4">
          <div id="click-area"
          @mouseover="squareOut($event)" 
          @contextmenu="squareClick($event)" @click="squareClick($event)" 
          style="border: solid; height: 400px; width: 800px;">
            <h3 style="margin-top: 170px;">Кликни или наведи на меня!</h3>
          </div>
        </b-col>
      </b-row>
      <b-row>
        <b-col md="12">
           <b-modal
           title="Welcome"
           centered
           hide-footer
           id="success-modal">
              <h3>Привет, {{name}}!</h3>
              <h3>Роль: {{role}}</h3>
          </b-modal>

          <b-modal 
           title="Авторизация"
           centered
           no-close-on-esc 
           no-close-on-backdrop 
           hide-footer
           hide-header-close
           id="auth-modal">
           <b-form @submit.prevent="signIn">
            <b-input v-model="user['email']" required type="email" class="mb-3" placeholder="Email"></b-input>
            <b-input v-model="user['password']" required type="password" placeholder="Пароль"></b-input>
            <b-button class="mt-3" type="submit" block variant="success"> Войти </b-button>
            <b-button @click="showRegisterModal()" type="submit" class="mt-3" block variant="primary"> Регистрация </b-button>
           </b-form>
          </b-modal>

          <b-modal 
           title="Регистрация"
           centered
           no-close-on-esc 
           no-close-on-backdrop 
           hide-footer
           hide-header-close
           id="reg-modal">
           <b-form @submit.prevent="registerUser">
            <b-input v-model="user['email']" required type="email" class="mb-3" placeholder="Email"></b-input>
            <b-input v-model="user['password']" required type="password" class="mb-3" placeholder="Пароль"></b-input>
            <b-input v-model="user['name']" required class="mb-3" placeholder="Имя"></b-input>
            <b-input v-model="user['age']" required class="mb-3" placeholder="Возраст"></b-input>
            <b-input v-model="user['skype']" required class="mb-3" placeholder="Skype"></b-input>
            <b-input v-model="user['phoneNumber']" required class="mb-3" placeholder="Контакт"></b-input>
            <b-input v-model="user['whatsapp']" required class="mb-3" placeholder="WhatsApp"></b-input>
            <b-button class="mt-3" type="submit" block variant="success"> Зарегистрировать </b-button>
            <b-button class="mt-3" @click="onAuth()" block variant="primary"> Назад </b-button>
            </b-form>
          </b-modal>
          <b-modal
            centered
            hide-footer
            id="msg-modal">
              <h3>Отправить сообщение на почту?</h3>
              <b-button @click="sendMessageToEmail()" class="float-left" variant="primary">Да</b-button>
              <b-button @click="hideMessageModal()" class="float-right" variant="danger">Нет</b-button>
          </b-modal>
        </b-col>
      </b-row>
    </b-container>
  </div>
</template>

<script>
import axios from 'axios';
export default {
  name: 'App',
  components: {
  },
  data() {
    return {
      user: {},
      buttonVariant: 'success',
      buttonText: 'Запуск',
      name: '',
      role: '',
      filter: '',
      counter: 0,
      events: [],
      localTimeOptions: { hour: '2-digit', minute:'2-digit' }
    }
  },

  mounted() {
    let token = localStorage.getItem('token');
    if(token == null)
      this.$bvModal.show('auth-modal');

    this.getTableData();
  },

  computed: {
    filteredRecords: function() {
      var self = this;
      const filtered = this.events.filter(event => {
        let byDate = new Date(event.createdOn)
          .toLocaleDateString("ru-RU").indexOf(self.filter) > -1;
        let byTime = new Date(event.createdOn)
          .toLocaleTimeString([], this.localTimeOptions).indexOf(self.filter) > -1;
        let byEventType = event.eventType.indexOf(self.filter) > -1;

        if(byDate === true)
          return byDate;  
        if(byTime === true)
          return byTime;  
        if(byEventType === true)
        return byEventType;  
      });
      return filtered;
    }
  },

  methods: {
   getTableData: function() {
     if(localStorage.getItem('token') == null)
      return;

     axios.get('https://localhost:5001/api/allevent', {
       headers: {'Authorization': `Bearer ${localStorage.getItem('token')}`}
     }).then(response => {
       if(response.status == 201 || response.status == 200) {
        this.events = response.data;
        this.counter = response.data.length;
       }
     });
   },  

   hideMessageModal() {
     this.$bvModal.hide('msg-modal');
   },

   sendMessageToEmail() {
     let name = localStorage.getItem('name'); 
     let user = {
       "Name" : name
     }
     axios.post('https://localhost:5001/api/email', user, {
       headers: {'Authorization': `Bearer ${localStorage.getItem('token')}`}
     }).then(response => {
       console.log(response.status);
       alert('Письмо отправлено на почту!');
     })
   },

   deleteRecordsFromdb() {
     axios.delete('https://localhost:5001/api/delete', {
       headers: {'Authorization': `Bearer ${localStorage.getItem('token')}`}
     }).then(response => {
       console.log(response.data);
       console.log(response.status);
       this.getTableData();
     });
   },

   showRegisterModal: function() {
     this.$bvModal.hide('auth-modal');
     this.$bvModal.show('reg-modal');
   },

   logout: function() {
      this.$bvModal.show('auth-modal');
      localStorage.removeItem('token');
   },

   onAuth: function() {
     this.$bvModal.show('auth-modal');
     this.$bvModal.hide('reg-modal');
   },
   
   registerUser: function() {
     var regExp = /^[1-9]+$/;

     if(!regExp.test(this.user.age))
      alert('Недопустимое значение возраста!');

     this.user.age = +this.user.age;
     console.log(this.user);
     axios.post('https://localhost:5001/api/register', this.user).then(response => {
      if(response.status == 201 || response.status == 200) {
       this.$bvModal.hide('reg-modal');
       this.$bvModal.show('auth-modal');
      }
     });
   },

   signIn: function() {
     axios.post('https://localhost:5001/api/signin', this.user).then(response => {
       if(response.status == 201 || response.status == 200) {
         console.log(response.data);
         this.user = response.data;

         localStorage.setItem('token', response.data.token);
         localStorage.setItem('name', response.data.findUser.name);
         localStorage.setItem('role', response.data.role);

         this.name = response.data.findUser.name;
         this.role = response.data.role;

         this.$bvModal.show('success-modal');
         this.$bvModal.hide('auth-modal');
       }
     });
   },

   startButtonLogInfo() {
     axios.post('https://localhost:5001/api/start', null, {
       headers: {'Authorization': `Bearer ${localStorage.getItem('token')}`}
     }).then(response => {
       console.log(response.status);
     });
   },

   stopButtonLogInfo() {
     axios.post('https://localhost:5001/api/stop', null, {
       headers: {'Authorization': `Bearer ${localStorage.getItem('token')}`}
     }).then(response => {
       console.log(response.status);
     });
   },

   startButton: function() {
    if(this.buttonVariant == 'success') {
      this.buttonVariant = 'danger';
      this.buttonText = 'Стоп';
      this.startButtonLogInfo();
      return;
    }

    this.buttonVariant = 'success';
    this.buttonText = 'Запуск';
    this.stopButtonLogInfo();
  },

   registerEvent: function(eventInfo) {
     let eventRegister = {
       'EventType': eventInfo.eventType,
       'Coords': eventInfo.coords
     }

     console.log(eventRegister);
     axios.post('https://localhost:5001/api/eventreg', eventRegister, {
       headers: {'Authorization': `Bearer ${localStorage.getItem('token')}`}
     }).then(response => {
       if(response.status == 200 || response.status == 204)
        this.counter++;
        if(this.counter % 50 == 0 && this.counter > 0)
          this.$bvModal.show('msg-modal');
     });
   }, 

   squareOut: function(event) {
     if(this.buttonVariant == 'danger') {
      let eventInfo = {};
      eventInfo = {eventType: 'Сдвиг', createdOn: Date.now(), coords: `${event.clientX} : ${event.clientY}`};
        this.registerEvent(eventInfo);
        this.events.push(eventInfo);
     }
   },
  
   squareClick: function(event) {
    if(this.buttonVariant == 'danger') {
      let eventInfo = {};
      if(event.which == 1) {
        eventInfo = {eventType: 'Клик левой', createdOn: Date.now(), coords: `${event.clientX} : ${event.clientY}`};
        this.registerEvent(eventInfo);
        this.events.push(eventInfo);
      }

      else if(event.which == 2) {
        event.preventDefault();
        this.counter++;
      }

      else if(event.which == 3) {
        event.preventDefault();
        eventInfo = {eventType: 'Клик правой', createdOn: Date.now(), coords: `${event.clientX} : ${event.clientY}`};
        this.registerEvent(eventInfo);
        this.events.push(eventInfo);
      }
   }
  } 
 }
}
</script>

<style>
.table-container {
  max-height: 400px;
  width: 350px;
  overflow-y: scroll;
}

#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 30px;
}
</style>

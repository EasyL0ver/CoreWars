import { decodeToken } from "react-jwt";

class UserStore{
 

    setToken(token){
        const decodedToken = decodeToken(token)

        this.userId = decodedToken['user-id']
        this.email = decodeToken['email']
        this.token = token

        console.log(this)

        sessionStorage.setItem("token", this.token)
        sessionStorage.setItem("email", this.email)
        sessionStorage.setItem("userid", this.userId)
    }

    refresh(){
        this.token = sessionStorage.getItem("token")
        this.userId = sessionStorage.getItem("userid")
        this.email = sessionStorage.getItem("email")
    }

}


export default new UserStore();
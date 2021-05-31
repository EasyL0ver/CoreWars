import { decodeToken } from "react-jwt";

class UserStore{
 

    setToken(token){
        const decodedToken = decodeToken(token)

        this.userId = decodedToken['user-id']
        this.token = token

        console.log(this)

        sessionStorage.setItem("token", this.token)
        sessionStorage.setItem("userid", this.userId)
    }

    refresh(){
        this.token = sessionStorage.getItem("token")
        this.userId = sessionStorage.getItem("userid")
    }

}


export default new UserStore();
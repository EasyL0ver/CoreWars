import { decodeToken } from "react-jwt";

class UserStore{
 

    setToken(token){
        const decodedToken = decodeToken(token)

        this.userId = decodedToken['user-id']

        console.log(this)
    }

}


export default new UserStore();
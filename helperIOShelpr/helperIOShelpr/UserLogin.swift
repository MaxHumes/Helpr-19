//
//  UserLogin.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/9/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit
import Foundation

class UserLogin: UIViewController {

    //var httpResponse: URLResponse? = nil
    //var userData: Data? = nil
    
    var token: Data? = nil
    
    
    var masterControl: MasterViewController? = nil
    //var detailControl: DetailViewController? = nil
    //var gesture: UITapGestureRecognizer? = /UITapGestureRecognizer(target: self, action: #selector(onLoginTapped(_:)))
    
    
    override func viewDidLoad() {
        //guard gesture != nil else {
          //  print("DYEH")
            //return
        //}
        print("LOADED")
        //addChild("LoginNavigator")
        super.viewDidLoad()

        // Do any additional setup after loading the view.
    }
    //self.gesture = UITapGestureRecognizer(target: self, action: #selector(onLoginTapped(_:)))
    //var gest = self.gesture as! String

    @IBOutlet weak var userText: UITextField!
    
    @IBOutlet weak var passwordText: UITextField!
    
   
    
    @IBAction func onLoginReleased(_ sender: UIButton) {
        performSegue(withIdentifier: "showDetail", sender: sender)
    }
    
        
    @IBAction func onLoginTapped(_ sender: UIButton) {
        
        let semaphore = DispatchSemaphore(value: 0)
        
        
        
        //let parameters = String(format: "{\n\t\"email\" : \"%s\",\n\t\"password\" : \"%s\"\n}", userText.text!, passwordText.text!)
       //let parameters = NSString(string: "{\n\t\"email\" : \"\(userText.text!)\",\n\t\"password\" : \"\(passwordText.text!)\"\n}")
       //let parameters = "{\n\t\"email\" : \"\(userText.text!)\",\n\t\"password\" : ////\"\(passwordText.text!)\"\n}"
       // print("****\(parameters)******")
        
        let parameters: [String: String] = ["email": userText.text!, "password": passwordText.text!]
        
        //var uint = String.Encoding.utf8
        //let postData = parameters.data(using: String.Encoding(rawValue: String.Encoding.utf8.rawValue))
        //print(" &&&&& \(String(describing: postData))")
        guard let httpBody = try? JSONSerialization.data(withJSONObject: parameters, options: []) else {
            return
        }
        
        guard let url = URL(string: "https://helpr19api.azurewebsites.net/api/users/login") else { return }
        
        var request = URLRequest(url: url, timeoutInterval: Double.infinity)
        request.httpMethod = "POST"
        request.httpBody = httpBody
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        print(request.allHTTPHeaderFields!.count)
        print("$$$$")
        for key in request.allHTTPHeaderFields!.keys {
            print("\(key): \(String(describing: request.allHTTPHeaderFields![key]))")
        }
        print("$$$$")
        
        let task = URLSession.shared.dataTask(with: request) { data, response, error in
            guard let data = data else {
                print(String(describing: error))
                return
            }
            
            guard let response = (response as! HTTPURLResponse?) else {
                print(String(describing: error))
                return
            }
            print(String(data: data, encoding: .utf8)!)
            print("\n\n\nresponse: \(String(describing: response))")
            self.token = data
            semaphore.signal()
        }
        
        //print("response: \(response)")
        
        task.resume()
        semaphore.wait()
        /*
        let session = URLSession.shared
        session.dataTask(with: request) { (data, response, error) in
            if let response = response {
                self.httpResponse = response
                print(response)
            }
            
            if let newData = data {
                do {
                        //let json: NSString
                    let json = try JSONSerialization.jsonObject(with: newData, options: [])
                    print(json)
                    semaphore.signal()
                
                } catch {
                        
                    print("ERROR")
                }
                    //self.userData = data
            }
            
        }.resume()
        
        semaphore.wait()
        */
 
    } 
    //}
    
    
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
       if segue.identifier == "postLogin" {
            
            //addChild(segue.destination)
            //let splitController = segue.destination as! UISplitViewController
            //let jawn = splitController.viewControllers[0]
            //let jawn2 = splitController.viewControllers[1]
            masterControl = (segue.destination as? UINavigationController)?.topViewController as? MasterViewController
            masterControl!.token = self.token
           // detailControl = (splitController.viewControllers[1] as! UINavigationController).topViewController as? DetailViewController
            //detailControl?.managedObjectContext = masterControl?.managedObjectContext
        }
        
        
    }
    

}

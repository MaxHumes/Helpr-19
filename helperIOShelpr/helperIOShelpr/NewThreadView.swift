//
//  NewThreadView.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/5/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit
import Foundation

class NewThreadView: UIViewController {

    @IBOutlet weak var thread: UITextField!
    
    @IBOutlet weak var message: UITextField!
    
   /// var threadText: String = ""
    //var messageText: String = ""
    var wasRecentView: Bool = false
    
    @IBAction func onTouch(_ sender: Any) {
        //threadText = thread.text!/ messageText = message.text!
        
        let semaphore = DispatchSemaphore (value: 0)
        
        let parameters = ["name": thread.text!, "description": message.text!] as [String : Any]
        guard let httpBody = try? JSONSerialization.data(withJSONObject: parameters, options: []) else  {
            return
        }
        let url = URL(string: "https://helpr19api.azurewebsites.net/api/posts/add/thread")!
        var request = URLRequest(url: url, timeoutInterval: Double.infinity)
        request.httpMethod = "POST"
        request.httpBody = httpBody
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        request.addValue(String(data: token!, encoding: .utf8)!, forHTTPHeaderField: "token")
        
        let task = URLSession.shared.dataTask(with: request) { data, response, error in
            guard data != nil else {
                    print(String(describing: error))
                    return
                }
               
            guard (response as! HTTPURLResponse?) != nil else {
                    print(String(describing: error))
                    return
                }
              // print(String(data: data, encoding: .utf8)!)
               //print("\n\n\nresponse: \(String(describing: response))")
            }
            semaphore.signal()
        
        task.resume()
        semaphore.wait()
        task.suspend()
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()

    // Do any additional setup after loading the view.

    }
    
    override func viewWillDisappear(_ animated: Bool) {
       // wasRecentView = true
        super.viewWillDisappear(animated)

    }
 
    

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}
